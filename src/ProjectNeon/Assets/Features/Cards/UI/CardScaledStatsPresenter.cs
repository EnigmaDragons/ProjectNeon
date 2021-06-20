using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CardScaledStatsPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    public void Hide() => gameObject.SetActive(false);

    public void Show(CardTypeData c)
    {
        var scalingStats = c.BattleEffects().SelectMany(GetScalingStats).Distinct().ToArray();
        Show(c, scalingStats);
    }

    public void Show(CardTypeData c, string[] statTypes)
    {
        var statTypesString = string.Join(", ", statTypes);
        label.text = $"Scales with {statTypesString}";
        if (statTypes.Any())
            gameObject.SetActive(true);
    }

    private List<string> GetScalingStats(EffectData e)
    {
        var stats = new List<string>();
        var formula = e.Formula;
        foreach (var stat in Enum.GetValues(typeof(StatType)).Cast<StatType>())
            if (formula.IndexOf(stat.ToString(), StringComparison.OrdinalIgnoreCase) >= 0)
                stats.Add(stat.ToString());
        foreach (var stat in StatTypeAliases.AbbreviationToFullNames)
            if (formula.IndexOf(stat.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                stats.Add(stat.Value);
        return stats;
    }
}