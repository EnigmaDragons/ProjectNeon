using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class ResourceIcons
{
    private static object Owner = new object();
    private static readonly Dictionary<string, Regex> IconsRegex = new Dictionary<string, Regex>();
    private static readonly Dictionary<string, Regex> TermIconsRegex = new Dictionary<string, Regex>();
    private static HashSet<string> InitializedLanguages = new HashSet<string>();

    static ResourceIcons()
    {
        foreach (var kvp in Icons)
        {
            IconsRegex[kvp.Key] = new Regex(kvp.Key, RegexOptions.Compiled);
        }

        InitLanguage();
        Message.Subscribe<LanguageChanged>(OnLanguageChanged, Owner);
    }
    
    private static void OnLanguageChanged(LanguageChanged msg)
    {
        InitLanguage();
    }

    private static void InitLanguage()
    {
        foreach (var kvp in TermIcons)
        {
            TermIconsRegex[kvp.Key] = new Regex(kvp.Key.ToLocalized(), RegexOptions.Compiled);
        }
    }

    public static string ReplaceTextWithResourceIcons(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return str;
        
        foreach (var r in IconsRegex)
        {
            str = r.Value.Replace(str, Sprite[Icons[r.Key]]);
        }
        
        foreach (var r in TermIconsRegex)
        {
            str = r.Value.Replace(str, Sprite[TermIcons.VerboseGetValue(r.Key, nameof(TermIcons))]);
        }
        return str;
    }
    
    public static Dictionary<string, int> Icons = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
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

    private static string SpriteFn(int index) => $"<sprite index={index}>";
    private static Dictionary<int, string> Sprite = new Dictionary<int, string>()
    {
        { 1, SpriteFn(1) },
        { 2, SpriteFn(2) },
        { 3, SpriteFn(3) },
        { 4, SpriteFn(4) },
        { 5, SpriteFn(5) },
        { 6, SpriteFn(6) },
        { 7, SpriteFn(7) },
        { 8, SpriteFn(8) },
        { 9, SpriteFn(9) },
        { 10, SpriteFn(10) },
        { 11, SpriteFn(11) },
        { 12, SpriteFn(12) },
        { 13, SpriteFn(13) },
        { 14, SpriteFn(14) },
        { 15, SpriteFn(15) },
        { 16, SpriteFn(16) },
        { 17, SpriteFn(17) },
        { 18, SpriteFn(18) },
        { 19, SpriteFn(19) },
        { 20, SpriteFn(20) },
        { 21, SpriteFn(21) },
        { 22, SpriteFn(22) },
        { 23, SpriteFn(23) },
        { 24, SpriteFn(24) },
    };

    public static Dictionary<string, int> TermIcons = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
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