using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectCardTargetsV3 : OnMessage<BeginTargetSelectionRequested, EndTargetSelectionRequested, CancelTargetSelectionRequested>
{
    [SerializeField] private CardResolutionZone cardResolutionZone;
    [SerializeField] private CardPlayZone destinationCardZone;
    [SerializeField] private CardPlayZone sourceCardZone;
    [SerializeField] private BattleState battleState;
    [SerializeField] private BattlePlayerTargetingStateV2 targetingState;

    [ReadOnly, SerializeField] private Card card;
    
    protected override void Execute(BeginTargetSelectionRequested msg)
    {
        card = msg.Card;
        battleState.IsSelectingTargets = true;
        Message.Publish(new TargetSelectionBegun(card.Type));
        Log.Info($"UI - Began Target Selection for {card.Name}");
        InitCardForSelection(msg.Card);
    }

    protected override void Execute(EndTargetSelectionRequested msg) => EndSelection();
    protected override void Execute(CancelTargetSelectionRequested msg) => Cancel();

    public void EndSelection()
    {
        if (targetingState.HasValidTargets)
            Confirm();
        else 
            Cancel();
    }
    
    public void Cancel() => OnCancelled();
    public void OnCancelled()
    {
        Log.Info($"UI - Canceled Card {card?.Name}");
        Message.Publish(new PlayerCardCanceled());
        OnSelectionComplete();
    }

    public void Confirm()
    {
        if (card == null)
            Log.Error("Tried to Confirm Card but none was selected");
        PlayCard(new PlayedCardV2(card.Owner, targetingState.Targets, card));
    }

    private void PlayCard(PlayedCardV2 playedCard)
    {
        Debug.Log($"UI - Playing {card.Name} on {string.Join(" | ", targetingState.Targets.Select(x => x.ToString()))}");
        cardResolutionZone.Add(playedCard);
        Message.Publish(new PlayerCardSelected());
        sourceCardZone.Remove(card);
        destinationCardZone.PutOnBottom(card);
        OnSelectionComplete();
    }

    private void OnSelectionComplete()
    {
        card = null;
        battleState.IsSelectingTargets = false;
        Log.Info("UI - Target Selection Finished");
        Message.Publish(new TargetSelectionFinished());
    }

    private void InitCardForSelection(Card card)
    {
        targetingState.GetTargets = new List<Func<Target>>();
        targetingState.MemberToTargetMap = new List<Dictionary<int, Target>>();
        targetingState.TargetMember = Maybe<int>.Missing();
        var targetMaps = 0;
        foreach (var sequence in card.ActionSequences)
        {
            if (sequence.Group == Group.Self || sequence.Scope == Scope.All || sequence.Scope == Scope.AllExceptSelf)
            {
                var target = battleState.GetPossibleConsciousTargets(card.Owner, sequence.Group, sequence.Scope).First();
                targetingState.GetTargets.Add(() => target);
            }
            else
            {
                var tmp = targetMaps;
                targetingState.GetTargets.Add(() => targetingState.MemberToTargetMap[tmp][targetingState.TargetMember.Value]);
                var scopeOne = battleState.GetPossibleConsciousTargets(card.Owner, sequence.Group, Scope.One);
                targetingState.MemberToTargetMap.Add(battleState.GetPossibleConsciousTargets(card.Owner, sequence.Group, sequence.Scope)
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
    }
}
