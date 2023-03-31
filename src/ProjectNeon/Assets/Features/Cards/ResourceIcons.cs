using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class ResourceIcons
{
    public static string ReplaceTextWithResourceIcons(string str)
    {
        foreach (var r in Icons)
        {
            str = Regex.Replace(str, $" {r.Key}", $" {Sprite(r.Value)}");
            str = Regex.Replace(str, $"{r.Key}", $"{Sprite(r.Value)}");
        }
        foreach (var r in TermIcons)
        {
            var localized = r.Key.ToLocalized();
            str = Regex.Replace(str, $" {localized}", $" {Sprite(r.Value)}");
            str = Regex.Replace(str, $"{localized}", $"{Sprite(r.Value)}");
        }
        return str;
    }
    
    private static string Sprite(int index) => $"<sprite index={index}>";
    
    public static Dictionary<string, int> Icons = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase)
    {
        { "Ammo", 4 },
        { "Chems", 5 },
        { "Energy", 6 },
        { "Flames", 7 },
        { "Mana", 20 },
        { "Insight", 24 },
        { "Tech Points", 23 },
        { "TechPoints", 23 },
        { "PrimaryResource", 6 },
        { "Primary Resource", 6 },
        { "Grenades", 8 },
        { "Grenade", 8 },
        { "Ambition", 9 },
        { "Credits", 21},
        { "Creds", 21},
    };

    public static Dictionary<string, int> TermIcons => new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase)
    {
        { "Resources/Ammo", 4 },
        { "Resources/Chems", 5 },
        { "Resources/Energy", 6 },
        { "Resources/Flames", 7 },
        { "Resources/Mana", 20 },
        { "Resources/Insight", 24 },
        { "Resources/TechPoints", 23 },
        { "Resources/PrimaryResource", 6 },
        { "Resources/Grenades", 8 },
        { "Resources/Ambition", 9 },
        { "Resources/Creds", 21},
    };
}