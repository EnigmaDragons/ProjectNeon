using System.Collections.Generic;
using System.Linq;

public static class FloatExtensions
{
    private static readonly Dictionary<float, string> WholeNumbers = new Dictionary<float, string>();

    public static string GetCeilingIntString(this float f)
    {
        if (WholeNumbers.TryGetValue(f, out var str))
            return str;
        
        var s = f.CeilingInt().ToString();
        WholeNumbers[f] = s;
        return s;
    }

    public static void Init() => Enumerable.Range(0, 100).ForEach(i => GetCeilingIntString(i));
}
