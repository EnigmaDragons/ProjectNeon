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
    [SerializeField] private CardSpeed speed;
    [SerializeField] private ResourceCost cost;
    [SerializeField] private ResourceCost onPlayGain;
    [SerializeField] private Rarity rarity;
    [SerializeField] public CardActionSequence[] actionSequences = new CardActionSequence[0];
    [SerializeField] private CardType chainedCard;
    [SerializeField] private CardType swappedCard;
    [SerializeField] private string functionalityIssues;
    [SerializeField] private string presentationIssues;
    [SerializeField] private StringVariable[] archetypes;
    [SerializeField] private StaticCardCondition[] highlightCondition;
    [SerializeField] private StaticCardCondition[] unhighlightCondition;
    [SerializeField] private bool isWIP;

    public string Name => !string.IsNullOrWhiteSpace(customName) 
        ? customName 
        : name.SkipThroughFirstDash().SkipThroughFirstUnderscore().WithSpaceBetweenWords();
    
    public IResourceAmount Cost => cost;
    public IResourceAmount Gain => onPlayGain;
    public CardSpeed Speed => speed;
    public Sprite Art => art;
    public string Description => description;
    public HashSet<CardTag> Tags => new HashSet<CardTag>(tags);
    public string TypeDescription => typeDescription?.Value ?? "";
    public Rarity Rarity => rarity;
    public CardActionSequence[] ActionSequences => actionSequences == null ? new CardActionSequence[0] : actionSequences.ToArray();
    public CardActionsData[] Actions => ActionSequences.Select(a => a.CardActions).ToArray();
    public CardActionV2[] AllCardEffectSteps => Actions.SelectMany(a => a.Actions).ToArray(); 
    public Maybe<CardTypeData> ChainedCard => chainedCard;
    public Maybe<CardTypeData> SwappedCard => chainedCard;
    public HashSet<string> Archetypes => new HashSet<string>(archetypes.Select(x => x.Value));
    public string ArchetypeKey => string.Join(" + ", Archetypes.OrderBy(a => a));
    public bool IsWip => isWIP;
    public Maybe<CardCondition> HighlightCondition => highlightCondition != null && highlightCondition.Length > 0
        ? new Maybe<CardCondition>(new AndCardCondition(highlightCondition.Cast<CardCondition>().ToArray()))
        : Maybe<CardCondition>.Missing();
    public Maybe<CardCondition> UnhighlightCondition => unhighlightCondition != null && unhighlightCondition.Length > 0
        ? new Maybe<CardCondition>(new AndCardCondition(unhighlightCondition.Cast<CardCondition>().ToArray()))
        : Maybe<CardCondition>.Missing();

    public override string ToString() => Name;
    public override int GetHashCode() => ToString().GetHashCode();
    public override bool Equals(object other) => other is CardType && other.ToString() == ToString();

    public CardType Initialized(Rarity rarity)
    {
        this.rarity = rarity;
        return this;
    }
}
