using System.Collections.Generic;

public class DictionaryWithDefault<TKey, TValue> : Dictionary<TKey, TValue>
{
    private readonly TValue _defaultValue;

    public DictionaryWithDefault(TValue defaultValue, IEqualityComparer<TKey> equalityComparer = null)
        : base(equalityComparer ?? EqualityComparer<TKey>.Default)
        => _defaultValue = defaultValue;

    public DictionaryWithDefault(TValue defaultValue, IDictionary<TKey, TValue> values, IEqualityComparer<TKey> equalityComparer = null)
        : base(values, equalityComparer ?? EqualityComparer<TKey>.Default)
        => _defaultValue = defaultValue;

    public new TValue this[TKey key]
    {
        get => ContainsKey(key) ? base[key] : _defaultValue;
        set => base[key] = value;
    }
}
