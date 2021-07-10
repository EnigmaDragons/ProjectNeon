using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/Corp")]
public class StaticCorp : ScriptableObject, Corp
{
    [SerializeField] private StringVariable corpName;
    [SerializeField] private Sprite logo;
    [SerializeField] private Color color;
    [SerializeField] private StaticCorp[] rivalCorps;
    [SerializeField, TextArea(4, 10)] private string description;
    
    public string Name => corpName.Value;
    public Sprite Logo => logo;
    public Color Color => color;
    public string[] RivalCorpNames => rivalCorps.Select(r => r.Name).ToArray();
}