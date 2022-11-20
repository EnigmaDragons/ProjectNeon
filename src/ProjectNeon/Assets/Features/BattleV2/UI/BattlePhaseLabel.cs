using I2.Loc;
using TMPro;
using UnityEngine;

public class BattlePhaseLabel : OnMessage<BattleStateChanged>
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Localize labelLocalized;

    private void Awake()
    {
        label.horizontalAlignment = HorizontalAlignmentOptions.Left;
    }
    
    protected override void Execute(BattleStateChanged msg)
    {
        labelLocalized.SetTerm(msg.State.Phase == BattleV2Phase.NotBegun 
            ? string.Empty 
            : $"BattleUI/Phase {msg.State.Phase.ToString()}");
    }
}
