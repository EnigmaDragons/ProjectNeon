using UnityEngine;

public sealed class CardControlsPresenter : OnMessage<BattleStateChanged>
{
    [SerializeField] private BattleState state;
    [SerializeField] private GameObject cycleControl;
    [SerializeField] private GameObject discardControl;
    [SerializeField] private GameObject toggleBasicControl;
    [SerializeField] private GameObject basicSuperFocus;

    private bool _canCycleOrDiscard;
    
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        UpdateUi();
    }

    public void SetCanToggleBasic(bool canToggle)
    {
        if (toggleBasicControl != null)
            toggleBasicControl.SetActive(canToggle);
    }
    
    public void SetCanCycleOrDiscard(bool canCycleOrDiscard)
    {
        _canCycleOrDiscard = canCycleOrDiscard;
    }
    
    protected override void AfterEnable() => UpdateUi();
    protected override void Execute(BattleStateChanged msg) => UpdateUi();
    
    private void UpdateUi()
    {
        cycleControl.SetActive(_canCycleOrDiscard && state.NumberOfRecyclesRemainingThisTurn > 0);
        discardControl.SetActive(_canCycleOrDiscard && state.NumberOfRecyclesRemainingThisTurn == 0);
        basicSuperFocus.SetActive(state.BasicSuperFocusEnabled);
    }
}
