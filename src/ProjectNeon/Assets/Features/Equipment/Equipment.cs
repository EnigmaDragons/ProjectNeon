using System.Linq;

public interface Equipment
{
    int Id { get; }
    string Name { get; }
    string Description { get; }
    int Price { get; }
    Rarity Rarity { get; }
    string[] Archetypes { get; }
    EquipmentSlot Slot { get; }
    EquipmentStatModifier[] Modifiers { get; }
    IResourceType[] ResourceModifiers { get; }
    EffectData[] TurnStartEffects { get; }
    EffectData[] TurnEndEffects { get; }
    EffectData[] BattleStartEffects { get; }
    EffectData[] BattleEndEffects { get; }
    string Corp { get; }
    EquipmentDistributionRules DistributionRules { get; }
    Maybe<CardTypeData> ReferencedCard { get; }

    GameEquipmentData GetData();
}

public static class EquipmentExtensions
{
    public static StatAddends AdditiveStats(this Equipment e)
        => new StatAddends(e.Modifiers.Where(m => m.ModifierType == StatMathOperator.Additive)
            .ToDictionary(m => m.StatType, m => m.Amount));
    
    public static StatMultipliers MultiplierStats(this Equipment e)
        => new StatMultipliers(e.Modifiers.Where(m => m.ModifierType == StatMathOperator.Multiplier)
            .ToDictionary(m => m.StatType, m => m.Amount));

    public static string GetArchetypeKey(this Equipment e) => string.Join(" + ", e.Archetypes.OrderBy(a => a));

    public static bool IsRandomlyGenerated(this Equipment e) => e is InMemoryEquipment;
    public static bool IsImplant(this Equipment e) => e.Slot == EquipmentSlot.Permanent && e.Name.Equals("Implant");
    public static string GetMetricNameOrDescription(this Equipment e) => e.IsRandomlyGenerated() ? e.Description : e.Name;
    public static string GetInterpolatedDescription(this Equipment e)
    {
        return e.Description.Replace("true damage", InterpolatedCardDescriptions.TrueDamageIcon);
    }
}
