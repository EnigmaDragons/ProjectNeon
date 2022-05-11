using System.Collections.Generic;
using System.Linq;

public static class DictionaryExtensions
{
    public static Dictionary<TKey, TValue> With<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey keyToReplace, TValue replacement) where TKey : class
        => map.ToDictionary(x => x.Key, x => x.Key == keyToReplace ? replacement : x.Value);
    
    public static Dictionary<TKey, TValue> Without<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey keyToRemove) where TKey : class
        => map.Where(x => !EqualityComparer<TKey>.Default.Equals(x.Key, keyToRemove)).ToDictionary(x => x.Key, x => x.Value);
}