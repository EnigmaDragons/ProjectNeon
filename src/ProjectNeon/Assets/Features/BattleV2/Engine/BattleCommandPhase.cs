
using UnityEngine;

public class BattleCommandPhase : OnMessage<TargetSelectionBegun, TargetSelectionFinished>
{
    [SerializeField] private BattleUiVisuals ui;

    [SerializeField] private DirectionInputBinding directionBinding;
    [SerializeField] private VisualCardSelectionV2 cardSelection;
    [SerializeField] private BattlePlayerTargetingState targeting;
    
    public void Begin()
    {
        ui.BeginCommandPhase();
        directionBinding.Bind(cardSelection);
    }

    public void Wrapup()
    {
        ui.EndCommandPhase();
    }

    protected override void Execute(TargetSelectionBegun msg)
    {
        directionBinding.Bind(targeting);
    }

    protected override void Execute(TargetSelectionFinished msg)
    {
        directionBinding.Bind(cardSelection);
    }
}
