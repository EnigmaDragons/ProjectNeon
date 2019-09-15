using System;
using System.Collections.Generic;
using System.Linq;

public static class CollectionExtensions
{
    public static T[] AsArray<T>(this T item) => new [] {item};
    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action) => items.ToList().ForEach(action);
    public static IEnumerable<T> Concat<T>(this T item, IEnumerable<T> items)  => item.AsArray().Concat(items);
    public static IEnumerable<T> Concat<T>(this IEnumerable<T> items, T item) => items.Concat(item.AsArray());
    public static IEnumerable<T> Except<T>(this IEnumerable<T> items, T item) => items.Except(item.AsArray());
    public static bool None<T>(this IEnumerable<T> items) => !items.Any();
    public static IEnumerable<T> WrappedWith<T>(this IEnumerable<T> items, T wrapping) => wrapping.AsArray().Concat(items).Concat(wrapping);
}
