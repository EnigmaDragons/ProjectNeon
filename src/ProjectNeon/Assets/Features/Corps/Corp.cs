using UnityEngine;

public interface Corp
{
    string Name { get; }
    CorpGearShopData GearShopData { get; }
    string ClinicName { get; }
    Sprite ClinicImage { get; }
    Sprite Logo { get; }
    Color Color1 { get; }
    Color Color2 { get; }
    string[] RivalCorpNames { get; }
    StatType[] GeneratedEquipmentPrimaryStatPreference { get; }
    string ShortDescription { get; }
    string LongDescription { get; }
}

public static class CorpExtensions
{
    public static string GetLocalizedName(this Corp c) => c.GetTerm().ToLocalized();
    public static string GetTerm(this Corp c) => $"MegaCorps/{c.Name}";
}


