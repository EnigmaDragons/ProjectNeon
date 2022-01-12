using System.Collections.Generic;

public static class KeywordRules
{
    private static bool _isInitialized = false;
    
    // Rule Importance
    private static readonly DictionaryWithDefault<string, int> RulesByImportance = new DictionaryWithDefault<string, int>(0);

    public static readonly string Afflicted = "Afflicted";
    public static readonly string Bloodied = "Bloodied";
    
    public static readonly string Buyout = "Buyout";
    public static readonly string Chain = "Chain";
    public static readonly string Drain = "Drain";
    public static readonly string Focus = "Focus";
    public static readonly string Glitch = "Glitch";
    
    public static readonly string Igniting = "Igniting";
    public static readonly string Injure = "Injure";
    
    public static readonly string PrimaryStat = "PrimaryStat";
    public static readonly string Quick = "Quick";
    public static readonly string SelfDestruct = "SelfDestruct";
    
    private static readonly string[] RulesByImportanceArr = {
        Injure,
        SelfDestruct,
        Drain,
        Buyout,
        Afflicted,
        Igniting,
        Bloodied,
        "Sneaky",
        "Profitable",
        "Defenseless",
        TemporalStatType.Marked.ToString(),
        TemporalStatType.DoubleDamage.ToString(),
        TemporalStatType.Stun.ToString(),
        TemporalStatType.Aegis.ToString(),
        TemporalStatType.Blind.ToString(),
        TemporalStatType.Inhibit.ToString(),
        TemporalStatType.Marked.ToString(),
        TemporalStatType.Dodge.ToString(),
        TemporalStatType.Taunt.ToString(),
        TemporalStatType.Disabled.ToString(),
        TemporalStatType.Stealth.ToString(),
        TemporalStatType.Lifesteal.ToString(),
        TemporalStatType.Confused.ToString(),
        ReactionConditionType.OnSlay.ToString(),
        Focus,
        Glitch,
        "TagPlayed",
        TemporalStatType.Vulnerable.ToString(),
        "Critical",
        PrimaryStat,
        PlayerStatType.CardCycles.ToString(),
        "Swap",
        "SingleUse",
        "X-Cost",
        Chain,
        Quick
    };

    public static int ImportanceOrder(string rule)
    {
        InitIfNeeded();
        return RulesByImportance[rule];
    }
    
    private static void InitIfNeeded()
    {
        if (_isInitialized)
            return;
        
        _isInitialized = true;
        if (RulesByImportance.Count != RulesByImportanceArr.Length)
        {
            RulesByImportance.Clear();
            RulesByImportanceArr.ForEachIndex((r, i) => RulesByImportance[r] = i);
        }
    }

    public static void AddAllDescriptionFoundRules(List<string> rulesToShow, string description)
    {
        rulesToShow.AddIf(SelfDestruct, description.ContainsAnyCase(SelfDestruct));
        rulesToShow.AddIf(SelfDestruct, description.ContainsAnyCase("Self-Destruct"));
        rulesToShow.AddIf(Injure, description.ContainsAnyCase(Injure));
        rulesToShow.AddIf(Injure, description.ContainsAnyCase("Injury"));
        rulesToShow.AddIf(Focus, description.ContainsAnyCase(Focus));
        rulesToShow.AddIf(Drain, description.ContainsAnyCase(Drain));
        rulesToShow.AddIf(Quick, description.ContainsAnyCase(Quick));
        rulesToShow.AddIf(Chain, description.ContainsAnyCase(Chain));
        rulesToShow.AddIf(Bloodied, description.ContainsAnyCase(Bloodied));
        rulesToShow.AddIf(Bloodied, description.ContainsAnyCase("Bloody"));
        rulesToShow.AddIf("Sneaky", description.ContainsAnyCase("Sneaky"));
        rulesToShow.AddIf("Profitable", description.ContainsAnyCase("profitable"));
        rulesToShow.AddIf("Defenseless", description.ContainsAnyCase("Defenseless"));
        rulesToShow.AddIf("Buyout", description.ContainsAnyCase("Buyout"));
        rulesToShow.AddIf("Critical", description.ContainsAnyCase("Crit"));
        rulesToShow.AddIf(TemporalStatType.Marked.ToString(), description.ContainsAnyCase("Mark"));
        rulesToShow.AddIf(TemporalStatType.Lifesteal.ToString(), description.ContainsAnyCase(TemporalStatType.Lifesteal.ToString()));
        rulesToShow.AddIf(Afflicted, description.ContainsAnyCase("Afflict"));
    }

    public static void AddAllMatchingEffectScopeRules(List<string> rulesToShow, EffectData e, params string[] scopes) 
        => scopes.ForEach(s => rulesToShow.AddIf(s, e.EffectScope != null && s.Equals(e.EffectScope.Value)));
}