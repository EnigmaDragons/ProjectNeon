using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class HeroHealth
{
    [SerializeField] private int missingHp;
    [SerializeField] private List<AdditiveStatInjury> additiveStatInjuries = new List<AdditiveStatInjury>();
    [SerializeField] private List<MultiplicativeStatInjury> multiplicativeStatInjuries = new List<MultiplicativeStatInjury>();

    private Func<IStats> _getCurrentStats;
    
    public int MissingHp => missingHp;
    public IEnumerable<IStats> AdditiveStats(StatType primaryStat) => additiveStatInjuries.Select(i => new StatAddends().WithRaw(i.Stat == "PrimaryStat" ? primaryStat.ToString() : i.Stat, i.Amount));
    public IEnumerable<IStats> MultiplicativeStats(StatType primaryStat) => multiplicativeStatInjuries.Select(i => new StatMultipliers().WithRaw(i.Stat == "PrimaryStat" ? primaryStat.ToString() : i.Stat, i.Amount));

    public IEnumerable<string> InjuryNames => AllInjuries
        .Select(x => x.InjuryName)
        .Distinct();

    public Dictionary<HeroInjury, int> InjuryCounts => AllInjuries
        .GroupBy(x => x.InjuryName)
        .ToDictionary(g => g.First(), g => g.Count());

    public IEnumerable<HeroInjury> AllInjuries => additiveStatInjuries.Cast<HeroInjury>()
        .Concat(multiplicativeStatInjuries);

    public HeroHealth() {}
    public HeroHealth(Func<IStats> getCurrentStats) => _getCurrentStats = getCurrentStats;

    public void Init(Func<IStats> getCurrentStats) => _getCurrentStats = getCurrentStats;
    public void Apply(AdditiveStatInjury injury) => additiveStatInjuries.Add(injury);
    public void Apply(MultiplicativeStatInjury injury) => multiplicativeStatInjuries.Add(injury);
    public void HealToFull() => missingHp = 0;
    public void SetHp(int hp)
    {
        var maxHp = _getCurrentStats().MaxHp();
        missingHp = Mathf.Clamp(maxHp - hp, 0, maxHp);
    }
    public void AdjustHp(int amount) => missingHp = Mathf.Clamp(missingHp - amount, 0, _getCurrentStats().MaxHp());

    public void HealInjuryByName(string name)
    {
        additiveStatInjuries.RemoveAll(i => i.Name.Value.Equals(name));
        multiplicativeStatInjuries.RemoveAll(i => i.Name.Value.Equals(name));
    }
}
