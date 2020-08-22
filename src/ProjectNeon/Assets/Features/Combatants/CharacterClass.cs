using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/CharacterClass")]
public class CharacterClass : ScriptableObject
{
    public static string All => "All";
    
    [SerializeField] private string className;
    [SerializeField] private CardType basicCard;
    [SerializeField] private Color tint;

    public string Name => className;

    public CardType BasicCard => basicCard;
    public Color Tint => tint;

    public CharacterClass Initialized(string className)
    {
        this.className = className;
        return this;
    }
}
