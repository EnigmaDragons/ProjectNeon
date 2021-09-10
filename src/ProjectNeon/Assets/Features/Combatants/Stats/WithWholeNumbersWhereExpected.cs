
using System.Collections.Generic;
using UnityEngine;

public class WithWholeNumbersWhereExpected : IStats
{
    private readonly IStats _inner;

    public WithWholeNumbersWhereExpected(IStats inner) => _inner = inner;

    private static readonly HashSet<StatType> WholeNumberStatTypes = new HashSet<StatType>
    {
        StatType.Armor,
        StatType.Attack,
        StatType.Leadership,
        StatType.Magic,
        StatType.Resistance,
        StatType.ExtraCardPlays,
        StatType.MaxHP,
        StatType.MaxShield,
        StatType.Economy,
    };
    
    public float this[StatType statType] => WholeNumberStatTypes.Contains(statType) 
            ? Mathf.CeilToInt(_inner[statType]) 
            : _inner[statType];

    public float this[TemporalStatType statType] => Mathf.CeilToInt(_inner[statType]);
    public IResourceType[] ResourceTypes => _inner.ResourceTypes;
}

public static class WholeNumberStatExtensions
{
    public static IStats WithWholeNumbersWhereExpected(this IStats s) => new WithWholeNumbersWhereExpected(s);
    
}
