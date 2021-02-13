using System.Linq;
using UnityEngine;

public class SelectCardTargetsV3 : OnMessage<BeginTargetSelectionRequested, ConfirmTargetSelectionRequested, CancelTargetSelectionRequested>
{
    [SerializeField] private CardResolutionZone cardResolutionZone;
    [SerializeField] private CardPlayZone destinationCardZone;
    [SerializeField] private CardPlayZone sourceCardZone;
    [SerializeField] private BattleState battleState;
    [SerializeField] private BattlePlayerTargetingState targetingState;

    [ReadOnly, SerializeField] private Card card;
    
    private int _actionIndex;
    private int _numActions;
    private Target[] _actionTargets;

    protected override void Execute(BeginTargetSelectionRequested msg)
    {
        card = msg.Card;
        battleState.IsSelectingTargets = true;
        Message.Publish(new TargetSelectionBegun(card.Type));
        Log.Info($"UI - Began Target Selection for {card.Name}");

        _actionIndex = 0;
        _numActions = card.ActionSequences.Length;
        _actionTargets = new Target[_numActions];
        if (_numActions == 0)
        {
            Log.Error($"Card {card.Name} has no Card Actions");
            OnTargetConfirmed(Group.All, Scope.All);
            return;
        }

        PresentPossibleTargets();
    }

    protected override void Execute(ConfirmTargetSelectionRequested msg) => Confirm();
    protected override void Execute(CancelTargetSelectionRequested msg) => Cancel();
    
    private void PresentPossibleTargets()
    {
        var action = card.ActionSequences[_actionIndex];
        var possibleTargets = battleState.GetPossibleConsciousTargets(card.Owner, action.Group, action.Scope == Scope.AllExcept ? Scope.One : action.Scope);
        targetingState.WithPossibleTargets(possibleTargets);
        Message.Publish(new SelectionPossibleTargetsAvailable(possibleTargets));
    }

    public void Cancel() => OnCancelled();
    public void OnCancelled()
    {
        Log.Info($"UI - Canceled Card {card.Name}");
        Message.Publish(new PlayerCardCanceled());
        OnSelectionComplete();
    }

    public void Confirm()
    {
        if (card == null)
            Log.Error("Tried to Confirm Card but none was selected");
        OnTargetConfirmed(card.ActionSequences[_actionIndex].Group, card.ActionSequences[_actionIndex].Scope);
    }

    public void OnTargetConfirmed(Group group, Scope scope)
    {
        if (card == null)
        {
            Log.Info("UI - Attempted to confirm target, but Card was missing.");
            return;
        }

        if (_actionTargets.Length == 0)
            PlayCard(new PlayedCardV2(card.Owner, new Target[] {new Single(card.Owner),}, card));

        if (scope != Scope.AllExcept)
            _actionTargets[_actionIndex] = targetingState.Current;
        else
            _actionTargets[_actionIndex] = battleState.GetPossibleConsciousTargets(card.Owner, group, scope)
                .First(target => target.Members.All(member => !member.Equals(targetingState.Current.Members[0])));
        targetingState.Clear();

        if (_actionIndex + 1 == _numActions)
            PlayCard(new PlayedCardV2(card.Owner, _actionTargets, card));
        else
        {
            _actionIndex++;
            PresentPossibleTargets();
        }
    }

    private void PlayCard(PlayedCardV2 playedCard)
    {
        Debug.Log($"UI - Playing {card.Name} on {string.Join(" | ", _actionTargets.Select(x => x.ToString()))}");
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
}
