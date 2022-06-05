using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectCardTargetsV3 : OnMessage<BeginTargetSelectionRequested, EndTargetSelectionRequested, CancelTargetSelectionRequested>
{
    [SerializeField] private CardResolutionZone cardResolutionZone;
    [SerializeField] private CardPlayZone sourceCardZone;
    [SerializeField] private CardPlayZone discardZone;
    [SerializeField] private BattleState battleState;
    [SerializeField] private BattlePlayerTargetingStateV2 targetingState;

    [ReadOnly, SerializeField] private Card card;
    
    protected override void Execute(BeginTargetSelectionRequested msg)
    {
        card = msg.Card;
        battleState.IsSelectingTargets = true;
        Message.Publish(new TargetSelectionBegun(card.Type));
        Log.Info($"UI - Began Target Selection for {card.Name}");
        InitCardForSelection();
    }

    protected override void Execute(EndTargetSelectionRequested msg) => EndSelection(msg.ShouldDiscard);
    protected override void Execute(CancelTargetSelectionRequested msg) => Cancel();

    private void EndSelection(bool shouldDiscard)
    {
        Log.Info($"UI - Finished Target Selection. Discard {shouldDiscard}");
        if (battleState.Phase != BattleV2Phase.PlayCards)
            Cancel();
        else if (shouldDiscard)
            DiscardCard();
        else if (targetingState.HasValidTargets && card != null && card.IsPlayable(battleState.Party, battleState.NumberOfCardPlaysRemainingThisTurn))
            PlayCard(new PlayedCardV2(card.Owner, targetingState.Targets, card, false));
        else 
            Cancel();
    }

    public void Cancel() => OnCancelled();
    public void OnCancelled()
    {
        if (card == null)
            return;
        
        Log.Info($"UI - Canceled Card {card?.Name ?? "None"}");
        card.TransitionTo(CardMode.Normal);
        Message.Publish(new PlayerCardCanceled());
        OnSelectionComplete();
    }

    private void DiscardCard()
    {
        Debug.Log($"UI - Discarded {card.Name}");
        
        sourceCardZone.Remove(card);
        discardZone.PutOnBottom(card.RevertedToStandard());
        battleState.RecordCardDiscarded();
        OnSelectionComplete();
    }

    private void PlayCard(PlayedCardV2 playedCard)
    {
        Debug.Log($"UI - Played {playedCard.Card.Name} on {string.Join(" | ", playedCard.Targets.Select(x => x.ToString()))}");
        
        cardResolutionZone.PlayImmediately(playedCard);
        Message.Publish(new PlayerCardSelected());
        sourceCardZone.Remove(card);
        OnSelectionComplete();
    }

    private void OnSelectionComplete()
    {
        card = null;
        battleState.IsSelectingTargets = false;
        Log.Info("UI - Target Selection Finished");
        Message.Publish(new TargetSelectionFinished());
    }

    private void InitCardForSelection()
    {
        var getTargets = new List<Func<Target>>();
        var memberToTargetMap = new List<Dictionary<int, Target>>();
        var targetMaps = 0;
        foreach (var sequence in card.ActionSequences)
        {
            if (sequence.Group == Group.Self || sequence.Scope == Scope.All || sequence.Scope == Scope.AllExceptSelf)
            {
                var target = battleState.GetPossibleConsciousTargets(card.Owner, sequence.Group, sequence.Scope).First();
                getTargets.Add(() => target);
            }
            else
            {
                var tmp = targetMaps;
                getTargets.Add(() =>
                {
                    if (memberToTargetMap[tmp].ContainsKey(targetingState.TargetMember.Value))
                        return memberToTargetMap[tmp][targetingState.TargetMember.Value];
                    Log.Error("Get Targets was asked for when the targets were invalid");
                    return memberToTargetMap[tmp].First().Value;
                });
                var scopeOne = battleState.GetPossibleConsciousTargets(card.Owner, sequence.Group, Scope.One);
                memberToTargetMap.Add(battleState.GetPossibleConsciousTargets(card.Owner, sequence.Group, sequence.Scope)
                    .ToDictionary(x =>
                    {
                        if (sequence.Scope == Scope.One || sequence.Scope == Scope.OneExceptSelf)
                            return x.Members[0].Id;
                        if (sequence.Scope == Scope.AllExcept)
                            return scopeOne.First(targeted => x.Members.All(member => member.Id != targeted.Members[0].Id)).Members[0].Id;
                        throw new Exception($"Scope {sequence.Scope} not supported for card {sequence.Group}");
                    }, x => x));
                targetMaps++;
            }
        }
        targetingState.Init(memberToTargetMap, getTargets);
    }
}
