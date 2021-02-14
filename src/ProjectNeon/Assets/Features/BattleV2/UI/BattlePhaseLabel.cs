using TMPro;
using UnityEngine;

public class BattlePhaseLabel : OnMessage<BattleStateChanged>
{
    [SerializeField] private TextMeshProUGUI label;
    
    protected override void Execute(BattleStateChanged msg)
    {
        label.text = msg.State.Phase == BattleV2Phase.NotBegun 
            ? "" 
            : $"{msg.State.Phase.ToString().WithSpaceBetweenWords()}";
    }
}
