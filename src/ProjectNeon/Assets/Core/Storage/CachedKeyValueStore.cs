using System;
using System.Collections.Generic;

public sealed class CachedKeyValueStore<TValue> : IKeyValueStore<TValue>
{
    private readonly Dictionary<string, TValue> _cache = new Dictionary<string, TValue>();
    private readonly IKeyValueStore<TValue> _inner;

    public CachedKeyValueStore(IKeyValueStore<TValue> inner) => _inner = inner;
    
    public TValue GetOrDefault(string key, Func<TValue> getDefaultValue)
    {
        if (_cache.TryGetValue(key, out var val))
            return val;
        
        var value = _inner.GetOrDefault(key, getDefaultValue);
        _cache[key] = value;
        return value;
    }

    public void Put(string key, TValue obj)
    {
        _cache[key] = obj;
        _inner.Put(key, obj);
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
        _inner.Remove(key);
    }

    public void Clear()
    {
        _cache.Clear();
        _inner.Clear();
    }
}
