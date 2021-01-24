using System.Collections.Generic;
using System.Linq;

public sealed class StatMultipliers : IStats
{
    private readonly DictionaryWithDefault<string, float> _values = new DictionaryWithDefault<string, float>(1);

    public StatMultipliers() : this(new DictionaryWithDefault<string, float>(1)) {}
    public StatMultipliers(Dictionary<string, float> values) : this(new DictionaryWithDefault<string, float>(1, values)) {}
    private StatMultipliers(DictionaryWithDefault<string, float> values) => _values = values;
    
    public float this[StatType statType]
    {
        get => _values[statType.ToString()];
        set => _values[statType.ToString()] = value;
    }

    public float this[TemporalStatType statType] 
    {
        get => _values[statType.ToString()];
        set => _values[statType.ToString()] = value;
    }

    public StatMultipliers With(StatType statType, float value)
    {
        this[statType] = value;
        return this;
    }

    public StatMultipliers WithRaw(string statType, float value)
    {
        _values[statType] = value;
        return this;
    }
    
    public IResourceType[] ResourceTypes { get; set; } = new IResourceType[0];
    public override string ToString() => string.Join(", ", _values.Select(kv => $"x{kv.Value} {kv.Key}"));
}
