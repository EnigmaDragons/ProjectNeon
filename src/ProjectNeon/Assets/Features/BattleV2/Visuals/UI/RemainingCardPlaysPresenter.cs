using TMPro;
using UnityEngine;

public class RemainingCardPlaysPresenter : OnMessage<BattleStateChanged>
{
    [SerializeField] private TextMeshProUGUI counter;
    [SerializeField] private BattleState state;
    
    private void Awake() => counter.text = "3";
    protected override void AfterEnable() => Render(state);
    protected override void Execute(BattleStateChanged msg) => Render(msg.State);
    
    private void Render(BattleState s)
        => counter.text = s.NumberOfCardPlaysRemainingThisTurn.ToString();
}
