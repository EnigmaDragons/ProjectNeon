using System;
using System.Linq;

public static class StringExtensions
{
    public static string WithSpaceBetweenWords(this string s) => string.Concat(s.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
}
