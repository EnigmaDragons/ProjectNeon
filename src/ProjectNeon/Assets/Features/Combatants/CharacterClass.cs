using UnityEngine;

public class CharacterClass : ScriptableObject
{
    [SerializeField] private CardType basicCard;
    [SerializeField] private Color tint;

    public string Name => name.SkipThroughFirstDash().WithSpaceBetweenWords();
    public CardType BasicCard => basicCard;
    public Color Tint => tint;
}
