
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HeroHealth
{
    [SerializeField] private int missingHp;
    [SerializeField] private List<IStats> additiveStatInjuries;
    [SerializeField] private List<IStats> multiplicativeStatInjuries;

    public int MissingHp => missingHp;
    public IEnumerable<IStats> AdditiveStats => additiveStatInjuries;
    public IEnumerable<IStats> MultiplicativeStats => multiplicativeStatInjuries;

    private readonly Func<IStats> _getCurrentStats;
    
    public HeroHealth(Func<IStats> getCurrentStats)
    {
        _getCurrentStats = getCurrentStats;
        additiveStatInjuries = new List<IStats>();
        multiplicativeStatInjuries = new List<IStats>();
    }
    
    public void HealToFull() => missingHp = 0;
    public void SetHp(int hp) => missingHp = _getCurrentStats().MaxHp() - hp;
    public void AdjustHp(int amount) => missingHp = Mathf.Clamp(missingHp - amount, 0, _getCurrentStats().MaxHp());
    public void Apply(AdditiveStatInjury injury) => additiveStatInjuries.Add(new StatAddends().WithRaw(injury.Stat, injury.Amount));
    public void Apply(MultiplicativeStatInjury injury) => multiplicativeStatInjuries.Add(new StatMultipliers().WithRaw(injury.Stat, injury.Amount));
}
