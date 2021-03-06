using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public sealed class ReactionCardType : ScriptableObject, CardTypeData
{
    [SerializeField] private string displayName;
    [PreviewSprite] [SerializeField] private Sprite art;
    [SerializeField] [TextArea(1, 12)] private string description;
    [SerializeField] private StringVariable[] archetypes;
    [SerializeField] private ResourceCost cost;
    [SerializeField] private ResourceCost onPlayGain;
    [SerializeField] private CardReactionSequence actionSequence;

    public string Name => string.IsNullOrWhiteSpace(displayName) 
        ? name.SkipThroughFirstDash().SkipThroughFirstUnderscore().WithSpaceBetweenWords() 
        : displayName;

    public int Id => -1;
    public Sprite Art => art;
    public string Description => description;
    public string TypeDescription => "Reaction";
    public IResourceAmount Cost => cost;
    public IResourceAmount Gain => onPlayGain;
    public CardSpeed Speed => CardSpeed.Standard;
    public CardReactionSequence ActionSequence => actionSequence;
    public HashSet<CardTag> Tags => new HashSet<CardTag>();
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
    
    public ReactionCardType Initialized(ResourceCost cost, ResourceCost gain, CardReactionSequence action)
    {
        this.cost = cost;
        this.onPlayGain = gain;
        actionSequence = action;
        return this;
    }
}
