using System;
using System.Collections.Generic;
using System.Linq;

public static class Rng
{
    public static readonly int Seed = NewSeed();
    
    private static readonly Random Instance = new Random(Seed);

    public static int NewSeed() => Guid.NewGuid().GetHashCode();
    
    [Obsolete("Non-Deterministic")] public static bool Bool() => Int(2) == 1;
    [Obsolete("Non-Deterministic")] public static bool Chance(double percent) => Instance.NextDouble() < percent;
    [Obsolete("Non-Deterministic")] public static int Int() => Int(int.MaxValue);
    [Obsolete("Non-Deterministic")] public static int Int(int max) => Instance.Next(max);
    [Obsolete("Non-Deterministic")] public static int Int(int min, int max) => Instance.Next(min, max);
    [Obsolete("Non-Deterministic")] public static float Float() => Convert.ToSingle(Dbl());
    [Obsolete("Non-Deterministic")] public static double Dbl() => Instance.NextDouble();
    [Obsolete("Non-Deterministic")] public static KeyValuePair<T, T2> Random<T, T2>(this Dictionary<T, T2> dictionary) => dictionary.ElementAt(Int(dictionary.Count));
    [Obsolete("Non-Deterministic")] public static T Random<T>(this T[] array) => array[Int(array.Length)];
    [Obsolete("Non-Deterministic")] public static T Random<T>(this List<T> list) => list[Int(list.Count)];
    [Obsolete("Non-Deterministic")] public static T Random<T>(this Array array) => (T) array.GetValue(Int(array.Length));
    [Obsolete("Non-Deterministic")] public static T Random<T>(this IEnumerable<T> items) => items.ToArray().Random();

    [Obsolete("Non-Deterministic")] 
    public static T DrawRandom<T>(this List<T> list)
    {
        var roll = Int(list.Count);
        var item = list[roll];
        list.RemoveAt(roll);
        return item;
    }

    [Obsolete("Non-Deterministic")]
    public static T[] Shuffled<T>(this T[] arr)
    {
        var shuffled = arr.ToArray();
        var i = shuffled.Length;
        while (i > 1) 
        {
            var k = Instance.Next(i--);
            var temp = shuffled[i];
            shuffled[i] = shuffled[k];
            shuffled[k] = temp;
        }
        return shuffled;
    }
    
    [Obsolete("Non-Deterministic")]
    public static List<T> Shuffled<T>(this List<T> list)
    {
        var shuffled = list.ToList();
        var i = shuffled.Count;
        while (i > 1) 
        {
            var k = Instance.Next(i--);
            var temp = shuffled[i];
            shuffled[i] = shuffled[k];
            shuffled[k] = temp;
        }
        return shuffled;
    }

    [Obsolete("Non-Deterministic")]
    public static T[] TakeRandom<T>(this IEnumerable<T> items, int number)
        => items
            .ToArray()
            .Shuffled()
            .Take(number)
            .ToArray();
}
