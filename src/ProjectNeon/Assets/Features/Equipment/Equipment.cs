using System.Linq;

public interface Equipment
{
    string Name { get; }
    string Description { get; }
    int Price { get; }
    Rarity Rarity { get; }
    string[] Classes { get; }
    EquipmentSlot Slot { get; }
    EquipmentStatModifier[] Modifiers { get; }
    EffectData[] TurnStartEffects { get; }
    EffectData[] TurnEndEffects { get; }
}

public static class EquipmentExtensions
{
    public static StatAddends AdditiveStats(this Equipment e)
        => new StatAddends(e.Modifiers.Where(m => m.ModifierType == StatMathOperator.Additive)
            .ToDictionary(m => m.StatType, m => m.Amount));
    
    public static StatMultipliers MultiplierStats(this Equipment e)
        => new StatMultipliers(e.Modifiers.Where(m => m.ModifierType == StatMathOperator.Multiplier)
            .ToDictionary(m => m.StatType, m => m.Amount));
}
