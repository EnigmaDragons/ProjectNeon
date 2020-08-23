using System.Collections.Generic;

public class PlayerStatAddends : IPlayerStats
{
    private readonly DictionaryWithDefault<string, float> _values;
    
    public PlayerStatAddends() : this(new DictionaryWithDefault<string, float>(0)) {}
    public PlayerStatAddends(Dictionary<string, float> values) : this(new DictionaryWithDefault<string, float>(0, values)) {}
    private PlayerStatAddends(DictionaryWithDefault<string, float> values) => _values = values;
    
    public float this[PlayerStatType statType]
    {
        get => _values[statType.ToString()];
        private set => _values[statType.ToString()] = value;
    }
    
    public PlayerStatAddends With(PlayerStatType statType, float value)
    {
        this[statType] = value;
        return this;
    }
}