using System;

public interface IKeyValueStore<TKey, TValue>
{
    TValue GetOrDefault(TKey key, Func<TValue> getDefaultValue);
    void Put(TKey key, TValue obj);
    void Remove(TKey key);
    void Clear();
}

public interface IKeyValueStore<T> : IKeyValueStore<string, T> {}

public static class KeyValueExtensions
{
    public static IKeyValueStore<T> Cached<T>(this IKeyValueStore<T> store) => new CachedKeyValueStore<T>(store);
    public static IKeyValueStore<object> AsTypeless<T>(this IKeyValueStore<T> store) => new AsObjectStore<T>(store);
    public static T GetOrDefault<T>(this IKeyValueStore<T> store, string key, T def) => store.GetOrDefault(key, () => def);
    public static T GetOrDefaultAsType<T>(this IKeyValueStore<object> store, string key, T def) => (T)store.GetOrDefault(key, () => def);
}
