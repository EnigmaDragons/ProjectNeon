using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/Corp")]
public class Corp : ScriptableObject
{
    [SerializeField] private StringVariable corpName;
    [SerializeField] private Sprite logo;
    [SerializeField] private Color color;

    public string Name => corpName.Value;
    public Sprite Logo => logo;
    public Color Color => color;
}