using System.Collections.Generic;

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

    public IResourceType[] ResourceTypes { get; set; } = new IResourceType[0];
}
