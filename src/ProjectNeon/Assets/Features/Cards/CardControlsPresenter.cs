using UnityEngine;

public sealed class CardControlsPresenter : OnMessage<BattleStateChanged>
{
    [SerializeField] private BattleState state;
    [SerializeField] private GameObject cycleControl;

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        UpdateUi();
    }
    
    protected override void AfterEnable() => UpdateUi();
    protected override void Execute(BattleStateChanged msg) => UpdateUi();
    private void UpdateUi() => cycleControl.SetActive(state.NumberOfRecyclesRemainingThisTurn > 0);
}
