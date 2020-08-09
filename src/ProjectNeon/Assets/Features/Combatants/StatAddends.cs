public sealed class StatAddends : IStats
{
    private readonly DictionaryWithDefault<string, float> _values = new DictionaryWithDefault<string, float>(0);

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

    public StatAddends With(IResourceType[] resourceTypes)
    {
        ResourceTypes = resourceTypes;
        return this;
    }
    
    public IResourceType[] ResourceTypes { get; set; } = new IResourceType[0];
}
