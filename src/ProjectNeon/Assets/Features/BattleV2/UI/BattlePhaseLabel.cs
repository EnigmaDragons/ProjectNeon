using System;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;

public class BattlePhaseLabel : OnMessage<BattleStateChanged>, ILocalizeTerms
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

    public string[] GetLocalizeTerms()
    {
        var results = new List<string>();
        foreach (BattleV2Phase phase in Enum.GetValues(typeof(BattleV2Phase)))
            results.Add($"BattleUI/Phase {phase.ToString()}");
        return results.ToArray();
    }
}
