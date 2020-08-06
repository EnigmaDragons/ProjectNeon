using TMPro;
using UnityEngine;

public sealed class CardRecyclePresenter : OnMessage<BattleStateChanged>
{
    [SerializeField] private TextMeshProUGUI counter;

    private void Awake() => counter.text = "2";

    protected override void Execute(BattleStateChanged msg) => counter.text = msg.State.NumberOfRecyclesRemainingThisTurn.ToString();
}
