using UnityEngine;

public class CardTrashPresenter : OnMessage<BattleStateChanged>
{
    [SerializeField] private GameObject panel;
    [SerializeField] private BattleState state;

    private void Awake() => Render();

    protected override void Execute(BattleStateChanged msg) => Render();

    private void Render()
    {
        panel.SetActive(state.NumberOfRecyclesRemainingThisTurn <= 0);
    }
}
