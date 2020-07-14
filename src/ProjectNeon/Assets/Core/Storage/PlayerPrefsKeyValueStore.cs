using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerPrefsKeyValueStore
{
    private static IKeyValueStore<object> WithFeatures<T>(IKeyValueStore<T> inner) => inner.Cached().AsTypeless();
    
    private readonly Dictionary<Type, IKeyValueStore<object>> _stores;

    public PlayerPrefsKeyValueStore() 
        : this(new Dictionary<Type, IKeyValueStore<object>>
        {
            {typeof(bool), WithFeatures(new PlayerPrefsBoolStore())},
            {typeof(int), WithFeatures(new PlayerPrefsIntStore())},
            {typeof(float), WithFeatures(new PlayerPrefsFloatStore())},
            {typeof(string), WithFeatures(new PlayerPrefsStringStore())},
        }) {}
    
    internal PlayerPrefsKeyValueStore(Dictionary<Type, IKeyValueStore<object>> storesByType) => _stores = storesByType;

    public bool Exists(string key) => PlayerPrefs.HasKey(key);
    public T GetOrDefault<T>(string key, T defaultValue) => ForType(typeof(T)).GetOrDefaultAsType(key, defaultValue);
    public void Put(string key, object obj) => ForType(obj.GetType()).Put(key, obj);
    public void Remove<T>(string key) => ForType(typeof(T)).Remove(key);
    public void Clear() => _stores.ForEach(x => x.Value.Clear());
    public void Update<T>(string key, T defaultValue, Func<T, T> getNewValue)
    {
        var existing = GetOrDefault(key, defaultValue);
        var newValue = getNewValue(existing);
        Put(key, newValue);
    }

    private IKeyValueStore<object> ForType(Type type)
    {
        if (_stores.TryGetValue(type, out var store))
            return store;
        throw new InvalidOperationException($"No integration exists with PlayerPrefs for type {type.Name}");
    }
}

public sealed class PlayerPrefsIntStore : IKeyValueStore<int>
{
    public bool Exists(string key) => PlayerPrefs.HasKey(key);
    public int GetOrDefault(string key, Func<int> getDefaultValue) => Exists(key) ? Get(key) : getDefaultValue();
    private int Get(string key) => PlayerPrefs.GetInt(key);
    public void Put(string key, int obj) => PlayerPrefs.SetInt(key, obj);
    public void Remove(string key) => PlayerPrefs.DeleteKey(key);
    public void Clear() => PlayerPrefs.DeleteAll();
}

public sealed class PlayerPrefsBoolStore : IKeyValueStore<bool>
{
    public bool Exists(string key) => PlayerPrefs.HasKey(key);
    public bool GetOrDefault(string key, Func<bool> getDefaultValue) => Exists(key) ? Get(key) : getDefaultValue();
    private bool Get(string key) => PlayerPrefs.GetInt(key) > 0;
    public void Put(string key, bool obj) => PlayerPrefs.SetInt(key, obj ? 1 : 0);
    public void Remove(string key) => PlayerPrefs.DeleteKey(key);
    public void Clear() => PlayerPrefs.DeleteAll();
}

public sealed class PlayerPrefsStringStore : IKeyValueStore<string>
{
    public bool Exists(string key) => PlayerPrefs.HasKey(key);
    public string GetOrDefault(string key, Func<string> getDefaultValue) => Exists(key) ? Get(key) : getDefaultValue();
    private string Get(string key) => PlayerPrefs.GetString(key);
    public void Put(string key, string obj) => PlayerPrefs.SetString(key, obj);
    public void Remove(string key) => PlayerPrefs.DeleteKey(key);
    public void Clear() => PlayerPrefs.DeleteAll();
}

public sealed class PlayerPrefsFloatStore : IKeyValueStore<float>
{
    public bool Exists(string key) => PlayerPrefs.HasKey(key);
    public float GetOrDefault(string key, Func<float> getDefaultValue) => Exists(key) ? Get(key) : getDefaultValue();
    private float Get(string key) => PlayerPrefs.GetFloat(key);
    public void Put(string key, float obj) => PlayerPrefs.SetFloat(key, obj);
    public void Remove(string key) => PlayerPrefs.DeleteKey(key);
    public void Clear() => PlayerPrefs.DeleteAll();
}

