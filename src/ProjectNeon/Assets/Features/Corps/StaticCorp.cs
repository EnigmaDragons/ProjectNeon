using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/Corp")]
public class StaticCorp : ScriptableObject, Corp, ILocalizeTerms
{
    [SerializeField] private StringVariable corpName;
    [SerializeField] public string gearShopName;
    [SerializeField] private CorpAffinityLines gearAffinityLines = new CorpAffinityLines();
    [SerializeField] public string clinicName;
    [SerializeField] private Sprite logo;
    [SerializeField] private Sprite clinicImage;
    [SerializeField] private Color color;
    [SerializeField] private Color color2;
    [SerializeField] private StaticCorp[] rivalCorps;
    [SerializeField] private StatType[] generatedEquipmentPrimaryStatPreference;
    [SerializeField, TextArea(2, 4)] public string shortDescription;
    [SerializeField, TextArea(4, 10)] private string description;
    [SerializeField, TextArea(4, 24)] public string longDescription;
    [SerializeField] private string[] slogans;

    private string _shopTerm => $"MegaCorps/{corpName.Value}_Shop";

    public string Name => corpName.Value;
    public CorpGearShopData GearShopData => new CorpGearShopData(_shopTerm, gearAffinityLines ?? new CorpAffinityLines());
    public string ClinicNameTerm => $"MegaCorps/{corpName.Value}_Clinic";
    public Sprite Logo => logo;
    public Sprite ClinicImage => clinicImage;
    public Color Color1 => color;
    public Color Color2 => color2;
    public string[] RivalCorpNames => rivalCorps.Select(r => r.Name).ToArray();
    public StatType[] GeneratedEquipmentPrimaryStatPreference => generatedEquipmentPrimaryStatPreference ?? new StatType[0];
    public string ShortDescriptionTerm => $"MegaCorps/{corpName.Value}_ShortDescription";
    public string LongDescriptionTerm => $"MegaCorps/{corpName.Value}_LongDescription";
    public string[] GetLocalizeTerms()
        => new [] { this.GetTerm(), _shopTerm, ClinicNameTerm, ShortDescriptionTerm, LongDescriptionTerm };
}