
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

    public int MissingHp => missingHp;
    public IEnumerable<IStats> AdditiveStats => additiveStatInjuries.Select(i => new StatAddends().WithRaw(i.Stat, i.Amount));
    public IEnumerable<IStats> MultiplicativeStats => multiplicativeStatInjuries.Select(i => new StatMultipliers().WithRaw(i.Stat, i.Amount));

    public IEnumerable<string> InjuryNames => additiveStatInjuries.Select(i => i.Name.Value)
        .Concat(multiplicativeStatInjuries.Select(i => i.Name.Value))
        .Distinct();

    private readonly Func<IStats> _getCurrentStats;
    
    public HeroHealth(Func<IStats> getCurrentStats)
    {
        _getCurrentStats = getCurrentStats;
    }
    
    public void HealToFull() => missingHp = 0;
    public void SetHp(int hp) => missingHp = _getCurrentStats().MaxHp() - hp;
    public void AdjustHp(int amount) => missingHp = Mathf.Clamp(missingHp - amount, 0, _getCurrentStats().MaxHp());
    public void Apply(AdditiveStatInjury injury) => additiveStatInjuries.Add(injury);
    public void Apply(MultiplicativeStatInjury injury) => multiplicativeStatInjuries.Add(injury);

    public void HealInjuryByName(string name)
    {
        additiveStatInjuries.RemoveAll(i => i.Name.Value.Equals(name));
        multiplicativeStatInjuries.RemoveAll(i => i.Name.Value.Equals(name));
    }
}
