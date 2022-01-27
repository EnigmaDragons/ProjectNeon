using UnityEngine;

public static class CommonNameExtensions
{
    public static string GetName(this ScriptableObject x, string customName) => !string.IsNullOrWhiteSpace(customName) 
        ? customName 
        : x.name.SkipThroughFirstDash().SkipThroughFirstUnderscore().WithSpaceBetweenWords();
}
