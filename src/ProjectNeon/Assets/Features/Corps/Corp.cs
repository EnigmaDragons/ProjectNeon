using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/Corp")]
public class Corp : ScriptableObject
{
    [SerializeField] private string name;
    [SerializeField] private Sprite logo;
    [SerializeField] private Color color;

    public string Name => name;
    public Sprite Logo => logo;
    public Color Color => color;
}