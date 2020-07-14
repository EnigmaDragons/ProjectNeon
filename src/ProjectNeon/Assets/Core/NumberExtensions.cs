using System;

public static class NumberExtensions
{
    public static int FlooredInt(this float v) => Convert.ToInt32(Math.Floor(v));
}
