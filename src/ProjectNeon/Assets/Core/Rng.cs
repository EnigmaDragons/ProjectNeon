using System;
using System.Collections.Generic;
using System.Linq;

public static class Rng
{
    private static readonly Random Instance = new Random(Guid.NewGuid().GetHashCode());

    public static bool Bool()
    {
        return Int(2) == 1;
    }

    public static int Int()
    {
        return Int(int.MaxValue);
    }

    public static int Int(int max)
    {
        return Instance.Next(max);
    }

    public static int Int(int min, int max)
    {
        return Instance.Next(min, max);
    }

    public static double Dbl()
    {
        return Instance.NextDouble();
    }

    public static KeyValuePair<T, T2> Random<T, T2>(this Dictionary<T, T2> dictionary)
    {
        return dictionary.ElementAt(Int(dictionary.Count));
    }

    public static T Random<T>(this T[] array)
    {
        return array[Int(array.Length)];
    }

    public static T Random<T>(this List<T> list)
    {
        return list[Int(list.Count)];
    }
}
