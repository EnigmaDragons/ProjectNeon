using System;
using System.Collections.Generic;
using System.Linq;

public static class CollectionExtensions
{    
    public static bool None<T>(this IEnumerable<T> items) => !items.Any();
    public static bool None<T>(this IEnumerable<T> items, Func<T, bool> condition) => !items.Any(condition);
    [Obsolete] public static void CopiedForEach<T>(this IEnumerable<T> items, Action<T> action) => items.ToList().ForEach(action);
    [Obsolete] public static IEnumerable<T> Concat<T>(this IEnumerable<T> items, T item) => items.Concat(item.AsArray());
    [Obsolete] public static IEnumerable<T> Concat<T>(this T item, IEnumerable<T> items) => items.Concat(item.AsArray());
    [Obsolete] public static IEnumerable<T> ConcatIf<T>(this IEnumerable<T> items, T item, Func<T, bool> condition) 
        => item != null && condition(item) ? items.Concat(item) : items;
    [Obsolete] public static IEnumerable<T> ConcatIfNotNull<T>(this IEnumerable<T> items, T item) 
        => item != null ? items.Concat(item) : items;
    [Obsolete] public static IEnumerable<T> Except<T>(this IEnumerable<T> items, T item) => items.Except(new[] {item});
    [Obsolete] public static List<T> With<T>(this IEnumerable<T> list, T item) => list.Concat(new[] {item}).ToList();
    [Obsolete] public static List<T> Without<T>(this IEnumerable<T> list, T item)
    {
        var copyList = list.ToList();
        copyList.Remove(item);
        return copyList;
    }
    
    public static T[] AsArray<T>(this T item) => new [] {item};
    public static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> d, TKey key, Func<TValue> getDefault) => d.TryGetValue(key, out var value) ? value : getDefault();

    public static TValue VerboseGetValue<TKey, TValue>(this IDictionary<TKey, TValue> d, TKey key, string collectionName)
    {
        if (!d.TryGetValue(key, out var value))
            throw new KeyNotFoundException($"{key} not found in {collectionName}. Found Keys are {string.Join(", ", d.Keys.Select(x => x.ToString()))}");
        return value;
    }

    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (var item in items) 
            action(item);
    }

    public static void ForEachIndex<T>(this T[] items, Action<T, int> action)
    {
        for (var i = 0; i < items.Length; i++)
            action(items[i], i);
    }
    
    public static TValue VerboseGetValue<TKey, TValue>(this IDictionary<TKey, TValue> d, TKey k, Func<TKey, string> context)
    {
        if (!d.TryGetValue(k, out var value))
            throw new KeyNotFoundException($"Entry not found for '{context(k)}'");
        return value;
    }

    public static Maybe<T> FirstOrMaybe<T>(this IEnumerable<T> items, Func<T, bool> condition) where T : class 
        => new Maybe<T>(items.FirstOrDefault(condition));
    
    public static T[] SwapItems<T>(this T[] items, int first, int second)
    {
        if (first == second)
            return items;
        
        var tmp = items[first];
        items[first] = items[second];
        items[second] = tmp;
        return items;
    }

    public static Dictionary<TKey, TValue> SafeToDictionary<T, TKey, TValue>(this IEnumerable<T> items,
        Func<T, TKey> keySelector, Func<T, TValue> valueSelector)
    {
        var dictionary = new Dictionary<TKey, TValue>();
        items.ForEach(i => dictionary[keySelector(i)] = valueSelector(i));
        return dictionary;
    }
}
