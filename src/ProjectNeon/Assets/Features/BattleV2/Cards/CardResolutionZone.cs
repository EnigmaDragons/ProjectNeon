using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    
    private List<IPlayedCard> _moves = new List<IPlayedCard>();

    public IEnumerable<IPlayedCard> Moves => _moves;
    
    public void Init()
    {
        physicalZone.Clear();
        _moves.Clear();
        isResolving = false;
    }

    public bool HasMore => _moves.Any();
    public int Count => _moves.Count();
    
    private bool CanChain => _moves.Select(m => m.Member.Id).Distinct().Count() == 1 && _moves.Last().Card.ChainedCard.IsPresent;

    public IEnumerator AddChainedCardIfApplicable()
    {
        if (!CanChain) yield break;
        
        var chainingMove = _moves.Last();
        var owner = chainingMove.Member;
        var card = chainingMove.Card.ChainedCard.Value;
        var targets = GetTargets(owner, card, chainingMove.Targets);

        Add(new PlayedCardV2(owner, targets, card.CreateInstance(battleState.GetNextCardId(), owner), true));
        Message.Publish(new PlayRawBattleEffect("ChainText", new Vector3(0, 0, 0)));
        yield return new WaitForSeconds(1.6f);
    }

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
    
    public void Add(IPlayedCard played)
    {
        battleState.RecordPlayedCard(played);
        if (played.Card.TimingType == CardTimingType.Hasty)
        {
            _moves.Insert(0, played); 
            physicalZone.PutOnTop(played.Card); 
        }
        else
        {
            _moves.Add(played);
            physicalZone.PutOnBottom(played.Card); 
        }
        played.Member.Apply(m =>
        {
            m.Lose(played.Spent);
            m.Gain(played.Gained);
        });
        DevLog.Write($"{played.Member.Name} Played {played.Card.Name} - Spent {played.Spent} - Gained {played.Gained}");
    }

    public void ExpirePlayedCards(Func<IPlayedCard, bool> condition)
    {
        var movesCopy = _moves.ToArray();
        for (var i = movesCopy.Length - 1; i > -1; i--)
        {
            var played = movesCopy[i];
            played.Card.ClearXValue();
            if (!condition(played)) continue;
            
            DevLog.Write($"Expired played card {played.Card.Name} by {played.Member.Name}");
            if (_moves.Count > i)
                _moves.RemoveAt(i);
            physicalZone.Remove(played.Card);
            if (played.Member.TeamType == TeamType.Party)
                playerHand.PutOnBottom(playerPlayArea.Take(i));
        }
    }
    
    public void RemoveLastPlayedCard()
    {
        Debug.Log("UI - Remove Last Played Card");
        if (_moves.None() || 
            battleState.Phase != BattleV2Phase.Command || 
            isResolving || 
            battleState.IsSelectingTargets) 
                return;
        
        var played = _moves.Last();
        played.Card.ClearXValue();

        DevLog.Write($"Canceled playing {played.Card.Name}");
        battleState.RemoveLastRecordedPlayedCard();
        _moves.RemoveAt(_moves.Count - 1);
        var card = physicalZone.Take(physicalZone.Count - 1);
        playerPlayArea.Take(playerPlayArea.Count - 1);
        playerHand.PutOnBottom(card);
        
        played.Member.Apply(m => m.LoseResource(played.Gained.ResourceType, played.Gained.Amount));
        played.Member.Apply(m => m.GainResource(played.Spent.ResourceType, played.Spent.Amount));
        Message.Publish(new PlayerCardCanceled());
    }

    public void BeginResolvingNext()
    {
        DevLog.Write("Requested Resolve Next Card");
        isResolving = true;
        var move = _moves[0];
        _moves = _moves.Skip(1).ToList();
        StartResolvingOneCard(move);
    }

    private void StartResolvingOneCard(IPlayedCard played)
    {
        Message.Publish(new CardResolutionStarted(played));
        BattleLog.Write($"Resolving {played.Member.Name}'s {played.Card.Name}");
        if (physicalZone.Count == 0)
            DevLog.Write($"Weird Physical Zone Draw bug.");
        else
            physicalZone.DrawOneCard();

        Async.ExecuteAfterDelay(delayBeforeResolving, () =>
        {
            var card = played.Card;
            if (card.Owner.IsStunnedForCard())
            {
                BattleLog.Write($"{card.Owner.Name} was stunned, so {card.Name} does not resolve.");
                card.Owner.Apply(m =>
                    m.ApplyTemporaryAdditive(
                        AdjustedStats.CreateIndefinite(new StatAddends().With(TemporalStatType.CardStun, -1), true)));
                WrapupCard(played, card);
                Message.Publish(new CardResolutionFinished(played.Member.Id));
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
        isResolving = _moves.Any();
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
}
