using System;
using System.Collections.Generic;
using System.Linq;

public static class IntExtensions
{
    private static readonly Dictionary<int, string> IntStrings = new Dictionary<int, string>();

    public static string GetString(this int n)
    {
        if (IntStrings.TryGetValue(n, out var str))
            return str;
        
        var s = n.ToString();
        IntStrings[n] = s;
        return s;
    }

    public static void Init() => Enumerable.Range(0, 100).ForEach(i => GetString(i));
}
