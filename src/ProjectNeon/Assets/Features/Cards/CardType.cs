using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Card")]
public class CardType : ScriptableObject, CardTypeData
{
    [SerializeField] private string customName;
    [PreviewSprite] [SerializeField] private Sprite art;
    [SerializeField] [TextArea(1, 12)] private string description;
    [SerializeField] private StringVariable typeDescription;
    [SerializeField] private CardTag[] tags;
    [SerializeField] private CharacterClass onlyPlayableByClass;
    [SerializeField] private ResourceCost cost;
    [SerializeField] private ResourceCost onPlayGain;
    [SerializeField] private Rarity rarity;
    [SerializeField] public CardActionSequence[] actionSequences = new CardActionSequence[0];
    [SerializeField] private CardType chainedCard;
    [SerializeField] private string functionalityIssues;

    public string Name => !string.IsNullOrWhiteSpace(customName) 
        ? customName 
        : name.SkipThroughFirstDash().SkipThroughFirstUnderscore().WithSpaceBetweenWords();
    
    public ResourceCost Cost => cost;
    public ResourceCost Gain => onPlayGain;
    public Sprite Art => art;
    public string Description => description;
    public HashSet<CardTag> Tags => new HashSet<CardTag>(tags);
    public string TypeDescription => typeDescription?.Value ?? "";
    public Rarity Rarity => rarity;
    public Maybe<CharacterClass> LimitedToClass => onlyPlayableByClass;
    public CardActionSequence[] ActionSequences => actionSequences == null ? new CardActionSequence[0] : actionSequences.ToArray();
    public CardActionsData[] Actions => ActionSequences.Select(a => a.CardActions).ToArray();
    public Maybe<CardTypeData> ChainedCard => chainedCard;

    public override string ToString() => Name;
    public override int GetHashCode() => ToString().GetHashCode();
    public override bool Equals(object other) => other is CardType && other.ToString() == ToString();
}
