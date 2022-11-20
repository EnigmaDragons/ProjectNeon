using TMPro;
using UnityEngine;

public sealed class CardPlaysPresenter : OnMessage<PlayerStateChanged>
{
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI counter;

    private void Awake() => counter.text = "3";

    protected override void Execute(PlayerStateChanged msg) 
        => counter.text = msg.Current.CurrentStats[PlayerStatType.CardPlays].ToString();
}
