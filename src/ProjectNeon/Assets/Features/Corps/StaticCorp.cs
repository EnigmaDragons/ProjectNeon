﻿using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/Corp")]
public class StaticCorp : ScriptableObject, Corp
{
    [SerializeField] private StringVariable corpName;
    [SerializeField] private string gearShopName;
    [SerializeField] private CorpAffinityLines gearAffinityLines = new CorpAffinityLines();
    [SerializeField] private string clinicName;
    [SerializeField] private Sprite logo;
    [SerializeField] private Sprite clinicImage;
    [SerializeField] private Color color;
    [SerializeField] private Color color2;
    [SerializeField] private StaticCorp[] rivalCorps;
    [SerializeField] private StatType[] generatedEquipmentPrimaryStatPreference;
    [SerializeField, TextArea(2, 4)] private string shortDescription;
    [SerializeField, TextArea(4, 10)] private string description;
    [SerializeField, TextArea(4, 24)] private string longDescription;
    [SerializeField] private string[] slogans;
    
    public string Name => corpName.Value;
    public CorpGearShopData GearShopData => new CorpGearShopData(gearShopName, gearAffinityLines ?? new CorpAffinityLines());
    public string ClinicName => clinicName;
    public Sprite Logo => logo;
    public Sprite ClinicImage => clinicImage;
    public Color Color1 => color;
    public Color Color2 => color2;
    public string[] RivalCorpNames => rivalCorps.Select(r => r.Name).ToArray();
    public StatType[] GeneratedEquipmentPrimaryStatPreference => generatedEquipmentPrimaryStatPreference ?? new StatType[0];
    public string ShortDescription => shortDescription;
    public string LongDescription => longDescription;
}