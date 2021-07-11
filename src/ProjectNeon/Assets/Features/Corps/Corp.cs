using UnityEngine;

public interface Corp
{
    string Name { get; }
    string GearShopName { get; }
    Sprite Logo { get; }
    Color Color1 { get; }
    Color Color2 { get; }
    string[] RivalCorpNames { get; }
    StatType[] GeneratedEquipmentPrimaryStatPreference { get; }
}
