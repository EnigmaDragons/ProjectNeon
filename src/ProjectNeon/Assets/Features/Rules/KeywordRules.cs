using System.Collections.Generic;

public static class KeywordRules
{
    private static bool _isInitialized = false;
    
    // Rule Importance
    private static readonly DictionaryWithDefault<string, int> RulesByImportance = new DictionaryWithDefault<string, int>(0);

    public const string Afflict = "Afflict";
    public const string Afflicted = "Afflicted";
    public const string Bloodied = "Bloodied";
    public const string Bloody = "Bloody";
    public const string Buyout = "Buyout";
    public const string Chain = "Chain";
    public const string Critical = "Critical";
    public const string Crit = "Crit";
    public const string Debuffs = "Debuffs";
    public const string Debuff = "Debuff";
    public const string Defenseless = "Defenseless";
    public const string Drain = "Drain";
    public const string Finisher = "Finisher";
    public const string Focus = "Focus";
    public const string Glitch = "Glitch";
    public const string Igniting = "Igniting";
    public const string Injure = "Injure";
    public const string Injury = "Injury";
    public const string Mark = "Mark";
    public const string Motionless = "Motionless";
    public const string PrimaryStat = "PrimaryStat";
    public const string Quick = "Quick";
    public const string ReStealth = "ReStealth";
    public const string ReStealthHyphen = "Re-Stealth";
    public const string SelfDestruct = "SelfDestruct";
    public const string SelfDestructHyphen = "Self-Destruct";
    public const string Sneaky = "Sneaky";
    public const string Profitable = "Profitable";
    public const string Vulnerable = "Vulnerable";
    
    private static readonly string[] RulesByImportanceArr = {
        Injure,
        SelfDestruct,
        Drain,
        Buyout,
        Afflicted,
        Igniting,
        Bloodied,
        Motionless,
        Sneaky,
        TemporalStatType.Prominent.ToString(),
        Finisher,
        Profitable,
        Defenseless,
        ReStealth,
        TemporalStatType.Marked.ToString(),
        TemporalStatType.DoubleDamage.ToString(),
        Vulnerable,
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
        Critical,
        PrimaryStat,
        StatType.Power.ToString(),
        PlayerStatType.CardCycles.ToString(),
        Debuffs,
        "Swap",
        "SingleUse",
        "X-Cost",
        Chain,
        Quick,
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
        void AddTerm(string term, params string[] aliases)
        {
            rulesToShow.AddIf(term, description.ContainsAnyCase(term));
            aliases.ForEach(a => rulesToShow.AddIf(term, description.ContainsAnyCase(a)));
        }
        
        AddTerm(Afflicted, Afflict);
        AddTerm(Bloodied, Bloody);
        AddTerm(Buyout);
        AddTerm(Chain, Finisher);
        AddTerm(Critical, Crit);
        AddTerm(Defenseless);
        AddTerm(Drain);
        AddTerm(Debuffs, Debuff);
        AddTerm(Focus);
        AddTerm(Injure, Injury);
        AddTerm(TemporalStatType.Lifesteal.GetString());
        AddTerm(TemporalStatType.Marked.GetString(), Mark);
        AddTerm(Motionless);
        AddTerm(StatType.Power.GetString());
        AddTerm(Profitable);
        AddTerm(TemporalStatType.Prominent.GetString());
        AddTerm(Quick);
        AddTerm(ReStealth, ReStealthHyphen);
        AddTerm(SelfDestruct, SelfDestructHyphen);
        AddTerm(Sneaky);
        AddTerm(Vulnerable);
    }

    public static void AddAllMatchingEffectScopeRules(List<string> rulesToShow, EffectData e, params string[] scopes) 
        => scopes.ForEach(s => rulesToShow.AddIf(s, e.EffectScope != null && s.Equals(e.EffectScope.Value)));
}
