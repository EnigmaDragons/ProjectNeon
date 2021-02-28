using TMPro;
using UnityEngine;

public sealed class CardRecyclePresenter : OnMessage<BattleStateChanged>
{
    [SerializeField] private GameObject panel;
    [SerializeField] private BattleState state;
    [SerializeField] private TextMeshProUGUI counter;

    private void Awake() => Render();

    protected override void Execute(BattleStateChanged msg) => Render();

    private void Render()
    {
        panel.SetActive(state.PlayerState.CardCycles > 0);
        counter.text = state.NumberOfRecyclesRemainingThisTurn.ToString();
    }
}
