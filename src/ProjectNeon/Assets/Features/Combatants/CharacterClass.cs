using UnityEngine;

public class CharacterClass : ScriptableObject
{
    [SerializeField] private CardType basicCard;
    [SerializeField] private Color tint;

    public string Name => _name ?? name.SkipThroughFirstDash().WithSpaceBetweenWords();
    public CardType BasicCard => basicCard;
    public Color Tint => tint;

    private string _name;

    public CharacterClass Initialized(string className)
    {
        _name = className;
        return this;
    }
}
