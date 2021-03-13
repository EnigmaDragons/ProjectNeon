using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Battle/CardResolutionZone")]
public class CardResolutionZone : ScriptableObject
{
    [SerializeField] private CardPlayZone playerHand;
    [SerializeField] private CardPlayZone playerPlayArea;
    [SerializeField] private CardPlayZone physicalZone;
    [SerializeField] private CardPlayZone playedDiscardZone;
    [SerializeField] private CardPlayZone currentResolvingCardZone;
    [SerializeField] private bool isResolving;
    [SerializeField] private BattleState battleState;
    [SerializeField] private FloatReference delayBeforeResolving = new FloatReference(0.3f);
    public IPlayedCard LastPlayed { get; set; }
    
    private List<IPlayedCard> _movesThisTurn = new List<IPlayedCard>();
    private List<IPlayedCard> _pendingMoves = new List<IPlayedCard>();
   
    public void Init()
    {
        physicalZone.Clear();
        _movesThisTurn.Clear();
        _pendingMoves.Clear();
        isResolving = false;
    }

    public bool HasMore => _pendingMoves.Any();
    public int NumPlayedThisTurn => _movesThisTurn.Count;
    
    private bool CanChain => _movesThisTurn.Select(m => m.Member.Id).Distinct().Count() == 1 && _movesThisTurn.Last().Card.ChainedCard.IsPresent;
    
    private Target[] GetTargets(Member m, CardTypeData card, Maybe<Target[]> previousTargets)
    {
        var targets = new Target[card.ActionSequences.Length];
        for (var i = 0; i < targets.Length; i++)
        {
            var action = card.ActionSequences[i];
            var possibleTargets = battleState.GetPossibleConsciousTargets(m, action.Group, action.Scope);
            targets[i] = possibleTargets.First();
            if (previousTargets.IsPresent)
                foreach(var previousTarget in previousTargets.Value)
                    if (possibleTargets.Contains(previousTarget))
                        targets[i] = previousTarget;
        }
        return targets;
    }

    public void NotifyTurnFinished()
    {
        _movesThisTurn.Clear();
    }
    
    public void Add(IPlayedCard played)
    {
        battleState.RecordPlayedCard(played);
        BattleLog.Write(played.Card.IsActive 
            ? $"{played.Member.Name} played {played.Card.Name}{TargetDescription(played)}"
            : $"{played.Member.Name} discarded {played.Card.Name}");
        if (played.Card.IsActive)
        {
            played.Member.Apply(m =>
            {
                m.Lose(played.Spent);
                m.Gain(played.Gained);
            });
            DevLog.Write($"{played.Member.Name} Played {played.Card.Name} - Spent {played.Spent} - Gained {played.Gained}"); 
        }

        if (isResolving)
            _pendingMoves.Add(played);
        else
            StartResolvingOneCard(played);
    }
    
    private string TargetDescription(IPlayedCard c)
    {
        var seq = c.Card.ActionSequences[0];
        if (seq.Scope == Scope.One)
            return $" on {c.Targets[0].Members[0].Name}";
        if (seq.Group == Group.Self)
            return "";
        if (seq.Group == Group.Ally && seq.Scope == Scope.All)
            return " on all Allies";
        if (seq.Group == Group.Opponent && seq.Scope == Scope.All)
            return " on all Enemies";
        return "";
    }

    public void ExpirePlayedCards(Func<IPlayedCard, bool> condition)
    {
        var movesCopy = _pendingMoves.ToArray();
        for (var i = movesCopy.Length - 1; i > -1; i--)
        {
            var played = movesCopy[i];
            played.Card.ClearXValue();
            if (!condition(played)) continue;
            
            DevLog.Write($"Expired played card {played.Card.Name} by {played.Member.Name}");
            if (_pendingMoves.Count > i)
                _pendingMoves.RemoveAt(i);
            if (played.Member.TeamType == TeamType.Party && playerPlayArea.HasCards)
                playerHand.PutOnBottom(playerPlayArea.Take(i));
        }
    }
    
    [Obsolete] public void RemoveLastPlayedCard() {}

    public void BeginResolvingNext()
    {
        DevLog.Write("Requested Resolve Next Card");
        isResolving = true;
        var move = _pendingMoves[0];
        _pendingMoves = _pendingMoves.Skip(1).ToList();
        StartResolvingOneCard(move);
    }

    private void StartResolvingOneCard(IPlayedCard played)
    {
        isResolving = true;
        var timingWord = played.IsInstant() ? "Instantly " : "";
        Message.Publish(new CardResolutionStarted(played));
        BattleLog.Write($"{timingWord}Resolving {played.Member.Name}'s {played.Card.Name}");
        
        Async.ExecuteAfterDelay(delayBeforeResolving, () =>
        {
            var card = played.Card;
            if (!card.IsActive)
            {
                BattleLog.Write($"{card.Name} does not resolve.");
                WrapupCard(played, card);
                Message.Publish(new CardResolutionFinished(played.Member.Id, false));
            }
            else if (card.Owner.IsStunnedForCard())
            {
                BattleLog.Write($"{card.Owner.Name} was stunned, so {card.Name} does not resolve.");
                card.Owner.State.Adjust(TemporalStatType.CardStun, -1);
                WrapupCard(played, card);
                Message.Publish(new CardResolutionFinished(played.Member.Id, played.IsInstant()));
            }
            else
            {
                currentResolvingCardZone.Set(card);
                played.Perform(battleState.GetSnapshot());
                WrapupCard(played, card);
            }
        });
    }

    private void WrapupCard(IPlayedCard played, Card physicalCard)
    {
        LastPlayed = played;
        if (played.Member.TeamType.Equals(TeamType.Party) && !played.IsTransient)
            playedDiscardZone.PutOnBottom(physicalCard.RevertedToStandard());
        isResolving = _pendingMoves.Any();
    }

    public IEnumerator AddBonusCardsIfApplicable()
    {
        var snapshot = battleState.GetSnapshot();
        var members = battleState.Members.Values.Where(x => x.IsConscious());
        foreach (var member in members)
        {
            var bonusCards = member.State.GetBonusCards(snapshot);
            foreach (var card in bonusCards)
            {
                if (!card.IsPlayableBy(member)) 
                    continue;
                
                var targets = GetTargets(member, card, Maybe<Target[]>.Missing());
                Add(new PlayedCardV2(member, targets, new Card(battleState.GetNextCardId(), member, card), isTransient: true));
                // Maybe Display cool Bonus Card text here
                yield return new WaitForSeconds(1.6f);
            }
        }
    }
    
    public IEnumerator AddChainedCardIfApplicable()
    {
        if (!CanChain) yield break;
        
        var chainingMove = _movesThisTurn.Last();
        var owner = chainingMove.Member;
        var card = chainingMove.Card.ChainedCard.Value;
        var targets = GetTargets(owner, card, chainingMove.Targets);

        Add(new PlayedCardV2(owner, targets, card.CreateInstance(battleState.GetNextCardId(), owner), true));
        Message.Publish(new PlayRawBattleEffect("ChainText", new Vector3(0, 0, 0)));
        yield return new WaitForSeconds(1.6f);
    }
}
