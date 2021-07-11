using UnityEngine;

public interface Corp
{
    string Name { get; }
    Sprite Logo { get; }
    Color Color1 { get; }
    Color Color2 { get; }
    string[] RivalCorpNames { get; }
    //May change to have corps generate their own equipment
    StatType[] GeneratedEquipmentPrimaryStatPreference { get; }
}
