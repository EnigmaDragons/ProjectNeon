using UnityEngine;

public class InMemoryCorp : Corp
{
    public string Name { get; set; } = "None";
    public CorpGearShopData GearShopData { get; set; } = new CorpGearShopData("None", new CorpAffinityLines());
    public string ClinicName { get; set; } = "None";
    public Sprite ClinicImage { get; set; } = null;
    public Sprite Logo { get; }
    public Color Color1 { get; } = Color.white;
    public Color Color2 { get; } = Color.black;
    public string[] RivalCorpNames { get; set; } = new string[0];
    public StatType[] GeneratedEquipmentPrimaryStatPreference { get; } = new StatType[0];
    public string ShortDescription { get; set; } = "None";

    public static implicit operator string(InMemoryCorp c) => c.Name;
}