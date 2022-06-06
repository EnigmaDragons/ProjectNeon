using TMPro;
using UnityEngine;

public class BattlePhaseLabel : OnMessage<BattleStateChanged>
{
    [SerializeField] private TextMeshProUGUI label;

    private void Awake()
    {
        label.horizontalAlignment = HorizontalAlignmentOptions.Left;
    }
    
    protected override void Execute(BattleStateChanged msg)
    {
        label.text = msg.State.Phase == BattleV2Phase.NotBegun 
            ? "" 
            : $"{msg.State.Phase.ToString().WithSpaceBetweenWords()}";
    }
}
