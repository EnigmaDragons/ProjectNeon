using System.Collections;
using System.Linq;
using UnityEngine;

public class BattleCommandPhase : OnMessage<TargetSelectionBegun, TargetSelectionFinished, PlayerTurnConfirmationStarted, PlayerTurnConfirmationAborted>
{
    [SerializeField] private BattleUiVisuals ui;
    [SerializeField] private BattleState state;
    [SerializeField] private CardResolutionZone resolutionZone;

    [SerializeField] private DirectionInputBinding directionBinding;
    [SerializeField] private VisualCardSelectionV2 cardSelection;
    [SerializeField] private BattlePlayerTargetingState targeting;
    
    [SerializeField] private ConfirmCancelBinding confirmCancelBinding;
    [SerializeField] private SelectCardTargetsV2 cardTargets;
    [SerializeField] private ConfirmPlayerTurnV2 turnConfirmation;
    
    public void Begin()
    {
        ui.BeginCommandPhase();
        directionBinding.Bind(cardSelection);
        confirmCancelBinding.Bind(cardSelection);
    }

    public IEnumerator Wrapup()
    {
        ui.EndCommandPhase();
        ChooseEnemyCards();
        yield return new WaitForSeconds(1);
    }

    private void ChooseEnemyCards()
    {
        state.Enemies
            .Where(e => e.IsConscious())
            .ForEach(e => Enumerable.Range(0, e.State.ExtraCardPlays())
                .ForEach(c => resolutionZone.Add(state.GetEnemyById(e.Id).AI.Play(e.Id))));
    }
    
    protected override void Execute(TargetSelectionBegun msg)
    {
        directionBinding.Bind(targeting);
        confirmCancelBinding.Bind(cardTargets);
    }

    protected override void Execute(TargetSelectionFinished msg) => UpdateConfirmationState();
    protected override void Execute(PlayerTurnConfirmationStarted msg) => UpdateConfirmationState();
    protected override void Execute(PlayerTurnConfirmationAborted msg) => UpdateConfirmationState();

    private void UpdateConfirmationState()
    {
        var readyForTurnConfirm = turnConfirmation.CanConfirm;
        if (readyForTurnConfirm)
        {
            directionBinding.Clear();
            confirmCancelBinding.Bind(turnConfirmation);
        }
        else
        {
            directionBinding.Bind(cardSelection);
            confirmCancelBinding.Bind(cardSelection);
        }
    }
}
