using System;
using System.Linq;

public static class StringExtensions
{
    public static string WithSpaceBetweenWords(this string s) => string.Concat(s.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
    public static string SkipThroughFirstDash(this string s) => s.Substring(s.IndexOf('-') + 1);
    public static string SkipThroughFirstUnderscore(this string s) => s.Substring(s.IndexOf('_') + 1);
    public static bool ContainsAnyCase(this string s, string term) => s.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0;
}
