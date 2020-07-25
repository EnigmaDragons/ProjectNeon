
using UnityEngine;

public class BattleCommandPhase : OnMessage<TargetSelectionBegun, TargetSelectionFinished>
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

    public void Wrapup()
    {
        ChooseEnemyCards();
        ui.EndCommandPhase();
    }

    private void ChooseEnemyCards()
    {
        state.Enemies
            .ForEach(e => resolutionZone.Add(state.GetEnemyById(e.Id).AI.Play(e.Id)));
    }
    
    protected override void Execute(TargetSelectionBegun msg)
    {
        directionBinding.Bind(targeting);
        confirmCancelBinding.Bind(cardTargets);
    }

    protected override void Execute(TargetSelectionFinished msg)
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
