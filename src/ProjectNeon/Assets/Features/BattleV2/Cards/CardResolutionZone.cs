using System;
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
    
    private List<IPlayedCard> _movesThisTurn = new List<IPlayedCard>();
    private List<IPlayedCard> _pendingMoves = new List<IPlayedCard>();
    private Maybe<IPlayedCard> _current = Maybe<IPlayedCard>.Missing();

    public Maybe<TeamType> CurrentTeamType => _current.Map(c => c.Member.TeamType);
    
    public void Init()
    {
        physicalZone.Clear();
        _movesThisTurn.Clear();
        _pendingMoves.Clear();
        isResolving = false;
    }

    public bool IsDone => !isResolving && !HasMore;
    public bool HasMore => _pendingMoves.Any();
    public int NumPlayedThisTurn => _movesThisTurn.Count;

    public void NotifyTurnFinished()
    {
        _movesThisTurn.Clear();
    }

    public void NotifyPlayerTurnEnded()
    {
        AddBonusCardsIfApplicable();
    }
    
    public void Queue(IPlayedCard played)
    {
        RecordCardAndPayCosts(played);
        Enqueue(played);
    }
    
    public void PlayImmediately(IPlayedCard played)
    {
        var shouldQueue = isResolving;
        RecordCardAndPayCosts(played);
        
        if (shouldQueue)
            Enqueue(played);
        else
            StartResolvingOneNonReactionCard(played);
        
        if (played.Member.TeamType == TeamType.Party && battleState.NumberOfCardPlaysRemainingThisTurn == 0)
            AddChainedCardIfApplicable(played);
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

    public void OnCardResolutionFinished()
    {
        _current.IfPresent(c => WrapupCard(c, c.Card));
        _current = Maybe<IPlayedCard>.Missing();
        currentResolvingCardZone.Clear();
    }
    
    public void BeginResolvingNext()
    {
        DevLog.Write("Requested Resolve Next Card");
        isResolving = true;
        var move = _pendingMoves[0];
        _pendingMoves = _pendingMoves.Skip(1).ToList();
        if (physicalZone.HasCards)
            physicalZone.DrawOneCard();
        StartResolvingOneNonReactionCard(move);
    }
    
    private void Enqueue(IPlayedCard card)
    {
        Log.Info($"Enqueued card {card.Card.Name}");
        physicalZone.PutOnBottom(card.Card);
        _pendingMoves.Add(card);
    }

    private void RecordCardAndPayCosts(IPlayedCard played)
    {
        battleState.RecordPlayedCard(played);
        BattleLog.Write(played.Card.IsActive 
            ? $"{played.Member.Name} played {played.Card.Name}{TargetDescription(played)}"
            : $"{played.Member.Name} discarded {played.Card.Name}");
        if (played.Card.IsActive)
        {
            played.Member.State.RecordUsage(played.Card);
            played.Member.Apply(m => m.Lose(played.Spent, battleState.Party));
            DevLog.Write($"{played.Member.Name} Played {played.Card.Name} - Spent {played.Spent}");
        }
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
    
    private void StartResolvingOneNonReactionCard(IPlayedCard played)
    {
        _movesThisTurn.Add(played);
        StartResolvingOneCard(played, () => played.Perform(battleState.GetSnapshot()));
    }

    public void StartResolvingOneCard(IPlayedCard played, Action perform)
    {
        isResolving = true;
        Message.Publish(new CardResolutionStarted(played));
        BattleLog.Write($"Resolving {played.Member.Name}'s {played.Card.Name}");

        _current = new Maybe<IPlayedCard>(played);
        var card = played.Card;
        if (card.IsActive && !card.Owner.IsStunnedForCard() && currentResolvingCardZone.TopCard.IsMissingOr(c => c != card))
            currentResolvingCardZone.Set(card);
        
        Async.ExecuteAfterDelay(delayBeforeResolving, () =>
        {
            if (!card.IsActive)
            {
                BattleLog.Write($"{card.Name} is {card.Mode}, so it does not resolve.");
                Message.Publish(new CardResolutionFinished(played));
            }
            else if (card.Owner.IsStunnedForCard())
            {
                BattleLog.Write($"{card.Owner.Name} was stunned, so {card.Name} does not resolve.");
                card.Owner.State.Adjust(TemporalStatType.Stun, -1);
                Message.Publish(new DisplayCharacterWordRequested(card.Owner, CharacterReactionType.Stunned));
                Message.Publish(new CardResolutionFinished(played));
            }
            else if (card.IsAttack && card.Owner.IsBlinded())
            {
                BattleLog.Write($"{card.Owner.Name} was blinded, so {card.Name} does not resolve.");
                card.Owner.State.Adjust(TemporalStatType.Blind, -1);
                Message.Publish(new DisplayCharacterWordRequested(card.Owner, CharacterReactionType.Blinded));
                Message.Publish(new CardResolutionFinished(played));
            }
            else if (!card.IsAttack && card.Owner.IsInhibited())
            {
                BattleLog.Write($"{card.Owner.Name} was inhibited, so {card.Name} does not resolve.");
                card.Owner.State.Adjust(TemporalStatType.Inhibit, -1);
                Message.Publish(new DisplayCharacterWordRequested(card.Owner, CharacterReactionType.Inhibited));
                Message.Publish(new CardResolutionFinished(played));
            }
            else
            {
                perform();
            }
        });
    }
    
    private void WrapupCard(IPlayedCard played, Card physicalCard)
    {
        Log.Info($"Wrapped Up {played.Card.Name}. Pending Cards {_pendingMoves.Count}");
        if (played.Member.TeamType.Equals(TeamType.Party) && !played.IsTransient && !played.IsSingleUse)
        {
            if (physicalCard.Type.SwappedCard.IsPresent)
            {
                playedDiscardZone.PutOnBottom(new Card(
                    battleState.GetNextCardId(), 
                    physicalCard.Owner, 
                    physicalCard.Type.SwappedCard.Value, 
                    physicalCard.OwnerTint, 
                    physicalCard.OwnerBust));
            }
            else
            {
                playedDiscardZone.PutOnBottom(physicalCard.RevertedToStandard());   
            }
        }
        played.Member.State.RemoveTemporaryEffects(x => x.Status.Tag == StatusTag.CurrentCardOnly);
        isResolving = _pendingMoves.Any();
    }

    private void AddBonusCardsIfApplicable()
    {
        Log.Info("Checking Bonus Cards");
        var snapshot = battleState.GetSnapshot();
        var members = battleState.Members.Values.Where(x => x.IsConscious());
        foreach (var member in members)
        {
            var bonusCards = member.State.GetBonusCards(snapshot);
            Log.Info($"Num Bonus Cards {member.Name}: {bonusCards.Length}");
            foreach (var card in bonusCards)
            {
                if (!card.IsPlayableBy(member, battleState.Party, 1)) 
                    continue;

                var lastPlayedMove = _movesThisTurn.LastOrMaybe();
                var targets = GetTargets(member, card, lastPlayedMove.Map(move => move.Targets));
                if (member.TeamType == TeamType.Party)
                    PlayImmediately(new PlayedCardV2(member, targets, new Card(battleState.GetNextCardId(), member, card, battleState.GetHeroById(member.Id).Tint, battleState.GetHeroById(member.Id).Bust), isTransient: true));
                else
                    PlayImmediately(new PlayedCardV2(member, targets, new Card(battleState.GetNextCardId(), member, card), isTransient: true));
                Message.Publish(new DisplayCharacterWordRequested(member, CharacterReactionType.BonusCardPlayed));
            }
        }
    }
    
    private void AddChainedCardIfApplicable(IPlayedCard trigger)
    {
        if (!CanChain.Evaluate(new CardConditionContext(trigger.Card, battleState, _pendingMoves)))
            return;
        
        var chainingMove = trigger;
        var owner = chainingMove.Member;
        var card = chainingMove.Card.ChainedCard.Value;
        var targets = GetTargets(owner, card, chainingMove.Targets);

        Queue(new PlayedCardV2(owner, targets, card.CreateInstance(battleState.GetNextCardId(), owner, chainingMove.Card.OwnerTint, chainingMove.Card.OwnerBust), true, ResourceCalculations.Free));
        Message.Publish(new DisplayCharacterWordRequested(owner, CharacterReactionType.ChainCardPlayed));
    }
    
    private Target[] GetTargets(Member m, CardTypeData card, Maybe<Target[]> previousTargets)
    {
        var targets = new Target[card.ActionSequences.Length];
        for (var i = 0; i < targets.Length; i++)
        {
            var action = card.ActionSequences[i];
            var possibleTargets = battleState.GetPossibleConsciousTargets(m, action.Group, action.Scope);
            if (possibleTargets.None())
                targets[i] = new NoTarget();
            else
            {
                targets[i] = possibleTargets.First();
                if (previousTargets.IsPresent)
                    foreach (var previousTarget in previousTargets.Value)
                        if (possibleTargets.Contains(previousTarget))
                            targets[i] = previousTarget;
            }
        }
        return targets;
    }
}
