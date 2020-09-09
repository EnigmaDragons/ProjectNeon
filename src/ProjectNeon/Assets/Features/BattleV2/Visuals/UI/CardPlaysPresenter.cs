using TMPro;
using UnityEngine;

public sealed class CardPlaysPresenter : OnMessage<BattleStateChanged>
{
    [SerializeField] private TextMeshProUGUI counter;

    private void Awake() => counter.text = "3";

    protected override void Execute(BattleStateChanged msg) 
        => counter.text = msg.State.PlayerState.CurrentStats[PlayerStatType.CardPlays].ToString();
}
