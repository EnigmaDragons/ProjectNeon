using UnityEngine;

public interface Corp
{
    string Name { get; }
    CorpGearShopData GearShopData { get; }
    string ClinicName { get; }
    Sprite Logo { get; }
    Color Color1 { get; }
    Color Color2 { get; }
    string[] RivalCorpNames { get; }
    StatType[] GeneratedEquipmentPrimaryStatPreference { get; }
    string ShortDescription { get; }
}
