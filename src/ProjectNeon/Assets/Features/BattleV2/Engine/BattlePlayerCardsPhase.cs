using System;
using System.Collections;
using UnityEngine;

[Obsolete]
public class BattlePlayerCardsPhase : OnMessage<TargetSelectionBegun, TargetSelectionFinished, PlayerTurnConfirmationStarted, PlayerTurnConfirmationAborted>
{
    [SerializeField] private BattleUiVisuals ui;

    [SerializeField] private DirectionInputBinding directionBinding;
    [SerializeField] private VisualCardSelectionV2 cardSelection;
    
    [SerializeField] private ConfirmCancelBinding confirmCancelBinding;
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
        yield return new WaitForSeconds(1);
    }
    
    protected override void Execute(TargetSelectionBegun msg) {}
    protected override void Execute(TargetSelectionFinished msg) => UpdateConfirmationState();
    protected override void Execute(PlayerTurnConfirmationStarted msg) => UpdateConfirmationState();
    protected override void Execute(PlayerTurnConfirmationAborted msg) => UpdateConfirmationState();

    private void UpdateConfirmationState()
    {
    }
}
