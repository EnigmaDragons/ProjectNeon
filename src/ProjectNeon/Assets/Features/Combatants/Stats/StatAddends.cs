using System.Collections.Generic;
using System.IO;
using System.Linq;

public sealed class StatAddends : IStats
{
    private readonly DictionaryWithDefault<string, float> _values;

    public StatAddends() : this(new DictionaryWithDefault<string, float>(0)) {}
    public StatAddends(Dictionary<string, float> values) : this(new DictionaryWithDefault<string, float>(0, values)) {}
    private StatAddends(DictionaryWithDefault<string, float> values) => _values = values;
    
    public float this[StatType statType]
    {
        get => _values[statType.GetString()];
        private set => _values[statType.GetString()] = value;
    }

    public float this[TemporalStatType statType] 
    {
        get => _values[statType.GetString()];
        private set => _values[statType.GetString()] = value;
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
        ResourceTypes = resourceTypes.Select(x => new InMemoryResourceType(x)).ToArray();
        return this;
    }

    public StatAddends WithRaw(string statType, float value)
    {
        if (string.IsNullOrWhiteSpace(statType))
            throw new InvalidDataException($"Cannot add blank Stat");
        
        _values[statType] = value;
        return this;
    }
    
    public InMemoryResourceType[] ResourceTypes { get; set; } = new InMemoryResourceType[0];
    
    public override string ToString() => string.Join(", ", _values.Select(kv => $"{Sign(kv.Value)}{kv.Value} {kv.Key}"));
    private string Sign(float val) => val > 0 ? "+" : "";
}
