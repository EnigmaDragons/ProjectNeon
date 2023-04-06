using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WithWholeNumbersWhereExpected : IStats
{
    private IStats _inner;

    public WithWholeNumbersWhereExpected() : this(new StatAddends()) {}
    public WithWholeNumbersWhereExpected(IStats inner) => Initialized(inner);

    public WithWholeNumbersWhereExpected Initialized(IStats inner)
    {
        _inner = inner;
        return this;
    }
    
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
    public InMemoryResourceType[] ResourceTypes => _inner.ResourceTypes;
}

public static class WholeNumberStatExtensions
{   
    private static int _poolIndex = 0;
    private static readonly WithWholeNumbersWhereExpected[] Pool = Enumerable.Range(0, 500).Select(_ => new WithWholeNumbersWhereExpected()).ToArray();
    
    private static WithWholeNumbersWhereExpected GetNext()
    {
        _poolIndex = (_poolIndex + 1) % Pool.Length;
        return Pool[_poolIndex];
    }

    public static bool Init = true;
    
    public static IStats WithWholeNumbersWhereExpected(this IStats s) => GetNext().Initialized(s);
}
