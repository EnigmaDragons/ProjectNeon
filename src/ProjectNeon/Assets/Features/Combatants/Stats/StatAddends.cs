using System.Collections.Generic;
using System.IO;

public sealed class StatAddends : IStats
{
    private readonly DictionaryWithDefault<string, float> _values ;

    public StatAddends() : this(new DictionaryWithDefault<string, float>(0)) {}
    public StatAddends(Dictionary<string, float> values) : this(new DictionaryWithDefault<string, float>(0, values)) {}
    private StatAddends(DictionaryWithDefault<string, float> values) => _values = values;
    
    public float this[StatType statType]
    {
        get => _values[statType.ToString()];
        private set => _values[statType.ToString()] = value;
    }

    public float this[TemporalStatType statType] 
    {
        get => _values[statType.ToString()];
        private set => _values[statType.ToString()] = value;
    }
    
    public StatAddends With(StatType statType, float value)
    {
        this[statType] = value;
        return this;
    }

    public StatAddends With(TemporalStatType statType, float value)
    {
        this[statType] = value;
        return this;
    }

    public StatAddends With(params IResourceType[] resourceTypes)
    {
        ResourceTypes = resourceTypes;
        return this;
    }

    public StatAddends WithRaw(string statType, float value)
    {
        if (string.IsNullOrWhiteSpace(statType))
            throw new InvalidDataException($"Cannot add blank Stat");
        
        _values[statType] = value;
        return this;
    }
    
    public IResourceType[] ResourceTypes { get; set; } = new IResourceType[0];
}
