using System.Collections.Generic;
using System.Linq;

public static class EquipmentExtensions
{
    public static StatMultipliers MultiplierStats(this StaticEquipment e)
        => new StatMultipliers(e.Modifiers.Where(m => m.ModifierType == StatMathOperator.Multiplier)
            .ToDictionary(m => m.StatType, m => m.Amount));
    
    public static bool IsImplant(this StaticEquipment e) => e.Slot == EquipmentSlot.Permanent && e.Name.Equals("Implant");
    public static string GetMetricNameOrDescription(this StaticEquipment e) => e.Name;
    public static string GetInterpolatedDescription(this StaticEquipment e)
    {
        return e.Description.Replace("true damage", InterpolatedCardDescriptions.TrueDamageIcon);
    }

    public static string GetArchetypeKey(this StaticEquipment e) => string.Join(" + ", e.Archetypes.OrderBy(a => a));
    
    public static string LocalizedArchetypeDescription(this StaticEquipment e) 
        => string.Join(" - ", e.Archetypes().Select(Localized.Archetype));
    
    private static List<string> Archetypes(this StaticEquipment e) =>
        e.Archetypes.AnyNonAlloc() 
            ? e.Archetypes.OrderBy(a => a).ToList() 
            : new List<string>{"General"};
    
    public static string LocalizationNameTerm(this StaticEquipment e)
        => $"EquipmentNames/{LocalizationNameKey(e)}";
    
    public static string LocalizationNameKey(this StaticEquipment e)
        => $"{e.Id.ToString().PadLeft(5, '0')}-Name";
    
    public static string LocalizationDescriptionTerm(this StaticEquipment e)
        => $"EquipmentDescriptions/{LocalizationDescriptionKey(e)}";
    
    public static string LocalizationDescriptionKey(this StaticEquipment e)
        => $"{e.Id.ToString().PadLeft(5, '0')}-Desc";
}
