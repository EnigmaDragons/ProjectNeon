using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(order = -98)]
public sealed class ReactionCardType : ScriptableObject, CardTypeData
{
    [SerializeField] private string displayName;
    [PreviewSprite] [SerializeField] private Sprite art;
    [SerializeField] [TextArea(1, 12)] private string description;
    [SerializeField] private CardDescriptionV2 descriptionV2;
    [SerializeField] private StringVariable[] archetypes;
    [SerializeField] private ResourceCost cost;
    [SerializeField] private CardReactionSequence actionSequence;
    [SerializeField] private CardTag[] tags;

    public string Name => string.IsNullOrWhiteSpace(displayName) 
        ? name.SkipThroughFirstDash().SkipThroughFirstUnderscore().WithSpaceBetweenWords() 
        : displayName;

    public int Id => -1;
    public Sprite Art => art;
    public string Description => description;
    public CardDescriptionV2 DescriptionV2 => descriptionV2;
    public string TypeDescription => "Reaction";
    public IResourceAmount Cost => cost;
    public CardSpeed Speed => CardSpeed.Standard;
    public CardReactionSequence ActionSequence => actionSequence;
    public HashSet<CardTag> Tags => new HashSet<CardTag>(tags);
    public CardActionSequence[] ActionSequences => new[] { CardActionSequence.ForReaction(ActionSequence.CardActions) };
    public Maybe<CardTypeData> ChainedCard => Maybe<CardTypeData>.Missing();
    public Maybe<CardTypeData> SwappedCard => Maybe<CardTypeData>.Missing();
    public Rarity Rarity => Rarity.Starter;
    public HashSet<string> Archetypes => new HashSet<string>(archetypes.Select(x => x.Value));
    public Maybe<CardCondition> HighlightCondition { get; } = Maybe<CardCondition>.Missing();
    public Maybe<CardCondition> UnhighlightCondition { get; } = Maybe<CardCondition>.Missing();
    public bool IsSinglePlay { get; } = true;

    public ReactionCardType Initialized(CardReactionSequence action)
    {
        actionSequence = action;
        return this;
    }
    
    public ReactionCardType Initialized(ResourceCost cost, CardReactionSequence action)
    {
        this.cost = cost;
        actionSequence = action;
        return this;
    }
}
