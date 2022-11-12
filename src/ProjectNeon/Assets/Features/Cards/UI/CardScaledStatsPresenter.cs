using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using UnityEngine;

// Minor Bug: Doesn't re-render text if Language changes while viewing card
public class CardScaledStatsPresenter : MonoBehaviour, ILocalizeTerms
{
    [SerializeField] private Localize label;

    public void Hide() => gameObject.SetActive(false);

    public void Show(CardTypeData c, StatType primaryStat)
    {
        var scalingStats = c.BattleEffects().SelectMany(e => GetScalingStats(e, primaryStat)).Distinct().ToArray();
        Show(scalingStats, primaryStat);
    }

    public void Show(string[] statTypes, StatType primaryStat)
    {
        if (!statTypes.AnyNonAlloc())
            label.SetTerm(NoScalingTerm);
        else
            label.SetFinalText($"{ScalesWithTerm.ToLocalized()} {string.Join(CommaJoin, statTypes)}");

        gameObject.SetActive(true);
    }

    private List<string> GetScalingStats(EffectData e, StatType primaryStat)
    {
        var stats = new List<string>();
        var formula = e.Formula;
        foreach (var stat in Enum.GetValues(typeof(StatType)).Cast<StatType>())
        {
            var impliedStat = stat == StatType.Power ? primaryStat : stat;
            if (formula.IndexOf($"Base[{stat}]", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Log.Info(impliedStat.GetString());
                Log.Info(impliedStat.GetLocalizedString());
                stats.Add($"{BaseTerm.ToLocalized()} {impliedStat.GetLocalizedString()}");
            }
            else if (formula.IndexOf(stat.GetString(), StringComparison.OrdinalIgnoreCase) >= 0)
                stats.Add(impliedStat.GetLocalizedString());
        }

        foreach (var stat in StatTypeAliases.AbbreviationToStat)
        {
            var impliedStat = stat.Value == StatType.Power ? primaryStat : stat.Value;
            if (formula.IndexOf($"Base[{stat.Key}]", StringComparison.OrdinalIgnoreCase) >= 0)
                stats.Add($"{BaseTerm.ToLocalized()} {impliedStat.GetLocalizedString()}");
            else if (formula.IndexOf(stat.Key, StringComparison.Ordinal) >= 0)
                stats.Add(impliedStat.GetLocalizedString());
        }

        return stats;
    }

    private const string CommaJoin = ", ";
    private const string BaseTerm = "BattleUI/Base";
    private const string NoScalingTerm = "BattleUI/No Scaling";
    private const string ScalesWithTerm = "BattleUI/Scales with";

    public string[] GetLocalizeTerms() => 
        Enum.GetValues(typeof(StatType)).Cast<StatType>().Select(s => s.GetTerm())
        .Concat(new[] { NoScalingTerm, ScalesWithTerm, BaseTerm })
        .ToArray();
}
