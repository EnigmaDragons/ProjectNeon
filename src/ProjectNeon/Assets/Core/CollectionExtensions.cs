using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class CollectionExtensions
{    
    public static bool None<T>(this IEnumerable<T> items) => !items.Any();
    public static bool None<T>(this IEnumerable<T> items, Func<T, bool> condition) => !items.Any(condition);
    [Obsolete] public static void CopiedForEach<T>(this IEnumerable<T> items, Action<T> action) => items.ToList().ForEach(action);
    [Obsolete] public static IEnumerable<T> Concat<T>(this IEnumerable<T> items, T item) => items.Concat(item.AsArray());
    [Obsolete] public static IEnumerable<T> Concat<T>(this T item, IEnumerable<T> items) => item.AsArray().Concat(items);
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
    public static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> d, TKey key, TValue defaultValue) => d.TryGetValue(key, out var value) ? value : defaultValue;
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
        => items.FirstOrDefault(condition);
    
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

    public static void RemoveLast<T>(this List<T> list) => list.RemoveAt(list.Count - 1);
    public static IEnumerable<T> TakeLast<T>(this List<T> list, int n) => list.Skip(Math.Max(0, list.Count - n));

    public static int Product<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
    {
        int result = 1;
        foreach (var item in source)
            result *= selector(item);
        return result;
    }
    
    public static IEnumerable<TSource> DistinctBy<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
        var set = new HashSet<TResult>();
        foreach(var item in source)
            if (set.Add(selector(item)))
                yield return item;
    }

    public static void AddIf<T>(this List<T> items, T item, bool condition)
    {
        if (condition)
            items.Add(item);
    }
    
    public static void AddIf<T>(this HashSet<T> items, T item, bool condition)
    {
        if (condition)
            items.Add(item);
    }

    public static Maybe<T> FirstAsMaybe<T>(this IEnumerable<T> items)
    {
        var maybeNext = items.Take(1).ToArray();
        return maybeNext.Any() ? new Maybe<T>(maybeNext[0]) : Maybe<T>.Missing();
    }
    
    public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> list, int depth)
    {
        if (depth == 1) 
            return list.Select(t => new T[] { t });
        
        return Permutations(list, depth - 1)
            .SelectMany(t => list.Where(e => !t.Contains(e)),
                (t1, t2) => t1.Concat(new T[] { t2 }));
    }
    
    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int depth)
    {
        var elem = elements.ToArray();
        var size = elem.Length;
 
        if (depth > size) 
            yield break;
 
        var numbers = new int[depth];
        for (var i = 0; i < depth; i++)
            numbers[i] = i;
        
        do
            yield return numbers.Select(n => elem[n]);
        while (NextCombination(numbers, size, depth));
    }
    
    private static bool NextCombination(IList<int> num, int n, int k)
    {
        bool finished;
        var changed = finished = false;
 
        if (k <= 0) 
            return false;
 
        for (var i = k - 1; !finished && !changed; i--)
        {
            if (num[i] < n - 1 - (k - 1) + i)
            {
                num[i]++;
                if (i < k - 1)
                    for (var j = i + 1; j < k; j++)
                        num[j] = num[j - 1] + 1;
                changed = true;
            }
            finished = i == 0;
        }
 
        return changed;
    }
}
