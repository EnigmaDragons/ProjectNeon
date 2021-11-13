using System;
using UnityEngine;

public sealed class CardControlsPresenter : OnMessage<BattleStateChanged>
{
    [SerializeField] private BattleState state;
    [SerializeField] private GameObject cycleControl;
    [SerializeField] private GameObject toggleBasicControl;

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
    
    protected override void AfterEnable() => UpdateUi();
    protected override void Execute(BattleStateChanged msg) => UpdateUi();
    private void UpdateUi() => cycleControl.SetActive(state.NumberOfRecyclesRemainingThisTurn > 0);
}
