using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/Corp")]
public class StaticCorp : ScriptableObject, Corp
{
    [SerializeField] private StringVariable corpName;
    [SerializeField] private Sprite logo;
    [SerializeField] private Color color;
    [SerializeField] private Color color2;
    [SerializeField] private StaticCorp[] rivalCorps;
    [SerializeField] private StatType[] generatedEquipmentPrimaryStatPreference;
    [SerializeField, TextArea(4, 10)] private string description;
    [SerializeField] private string[] slogans;
    
    public string Name => corpName.Value;
    public Sprite Logo => logo;
    public Color Color1 => color;
    public Color Color2 => color2;
    public string[] RivalCorpNames => rivalCorps.Select(r => r.Name).ToArray();
    public StatType[] GeneratedEquipmentPrimaryStatPreference => generatedEquipmentPrimaryStatPreference ?? new StatType[0];
}