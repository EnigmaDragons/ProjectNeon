using System;
using System.Linq;

public static class NumberExtensions
{
    public static int FlooredInt(this float v) => Convert.ToInt32(Math.Floor(v));
    public static int CeilingInt(this float v) => Convert.ToInt32(Math.Ceiling(v));
    public static bool IsFloatZero(this float v) => WithinEpsilon(v);
    public static int Clamped(this int v, int min, int max) => Math.Max(min, Math.Min(max, v));
    private static bool WithinEpsilon(float f) => Math.Abs(f) < 0.05;
    
    public static bool WentToZero(this int[] values) => values.First() > 0 && values.Last() == 0;
    public static bool Decreased(this int[] values) => values.Last() < values.First();
    public static bool Increased(this int[] values) => values.Last() > values.First();
    public static int Delta(this int[] values) => values.Last() - values.First();
}
