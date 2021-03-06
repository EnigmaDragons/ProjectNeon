using System.Collections.Generic;
using System.Linq;

public class BalanceEngine
{
    // For Most Cards
    private static readonly Dictionary<Rarity, float> RarityWorthFactor = new Dictionary<Rarity, float>
    {
        { Rarity.Starter, 0.8f },
        { Rarity.Basic, 1.0f },
        { Rarity.Common, 1.2f},
        { Rarity.Uncommon, 1.5f},
        { Rarity.Rare, 1.8f},
        { Rarity.Epic, 2.5f}
    };
    
    private static readonly Dictionary<Rarity, float> RarityFlatStat = new Dictionary<Rarity, float>
    {
        { Rarity.Starter, 10f },
        { Rarity.Basic, 10f },
        { Rarity.Common, 11f},
        { Rarity.Uncommon, 12f},
        { Rarity.Rare, 13f},
        { Rarity.Epic, 14f}
    };
    
    // For Multiplicative and Stuns/Blinds/Inhibits
    private static readonly Dictionary<Rarity, float> RarityPrimaryStat = new Dictionary<Rarity, float>
    {
        { Rarity.Starter, 10f },
        { Rarity.Basic, 10f },
        { Rarity.Common, 11f},
        { Rarity.Uncommon, 12f},
        { Rarity.Rare, 13f},
        { Rarity.Epic, 14f}
    };
    
    private static readonly Dictionary<Rarity, float> RaritySecondaryStat = new Dictionary<Rarity, float>
    {
        { Rarity.Starter, 8f },
        { Rarity.Basic, 8f },
        { Rarity.Common, 9f},
        { Rarity.Uncommon, 9f},
        { Rarity.Rare, 10f},
        { Rarity.Epic, 10f}
    };
    
    private static readonly Dictionary<EffectType, float> EffectTypeFlatFactor = new DictionaryWithDefault<EffectType, float>(1)
    {
    };

    private static float ScopeFactor(Scope s, Group g, params BalanceTag[] tags)
    {
        if (g == Group.Self && tags.Contains(BalanceTag.Harmful))
            return -0.5f;
        if (g == Group.Self && tags.Contains(BalanceTag.Resource))
            return 1f;
        if (g == Group.Self && tags.Contains(BalanceTag.StatBuff))
            return 1f;
        if (g == Group.Self)
            return 0.8f;
        if (s == Scope.One)
            return 1f;
        if (s == Scope.All && tags.Contains(BalanceTag.StatBuff))
            return 1.2f;
        if (s == Scope.All && tags.Contains(BalanceTag.Damage))
            return 1.8f;
        if (s == Scope.All)
            return 1.5f;
        return 1f;
    }

    private static readonly DictionaryWithDefault<string, float> ResourceTypeFactor = new DictionaryWithDefault<string, float>(0.5f)
    {
        {"None", 0f},
        {"Ammo", 0.2f},
        {"Flames", 0.33f},
        {"Energy", 0.33f},
        {"Chems", 0.4f},
    };

    private static float ResourceValue(IResourceAmount amt) 
        => amt.BaseAmount * ResourceTypeFactor[amt.ResourceType.Name];

    public static BalanceAssessment Assess(CardTypeData c)
    {
        var targetPower = RarityWorthFactor.VerboseGetValue(c.Rarity, "Rarity") + ResourceValue(c.Cost);
        var resourceGainValue = ResourceValue(c.Gain);
        var effectPowerLevel = c.ActionSequences.Sum(a => PowerLevel(c.Rarity, a));
        var actualPower = resourceGainValue + effectPowerLevel;
        return new BalanceAssessment(c.Name, targetPower, actualPower);
    }

    private static float PowerLevel(Rarity r, CardActionSequence seq)
    {
        var battleEffects = seq.CardActions.BattleEffects;
        var effectPowerLevel = battleEffects.Sum(b => PowerLevel(seq.Scope, seq.Group, r, b));
        return effectPowerLevel;
    }
    
    private static readonly HashSet<EffectType> DamageEffects = new HashSet<EffectType>
    {
        EffectType.AttackFormula,
        EffectType.DealRawDamageFormula,
        EffectType.MagicAttackFormula,
        EffectType.DamageOverTimeFormula,
    };
    
    private static BalanceTag[] ImpliedBalanceTags(EffectData e)
    {
        var isDamage = DamageEffects.Contains(e.EffectType);
        return isDamage ? new[] {BalanceTag.Damage} : new BalanceTag[0];
    }

    private static float PowerLevel(Scope s, Group g, Rarity r, EffectData e)
    {
        var scopeFactor = ScopeFactor(s, g, ImpliedBalanceTags(e));
        var effectValue = 1f;
        return effectValue * scopeFactor;
    }
}
