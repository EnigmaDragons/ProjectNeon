using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public sealed class ReactionCardType : ScriptableObject, CardTypeData
{
    [SerializeField] private string displayName;
    [PreviewSprite] [SerializeField] private Sprite art;
    [SerializeField] [TextArea(1, 12)] private string description;
    [SerializeField] private ResourceCost cost;
    [SerializeField] private ResourceCost onPlayGain;
    [SerializeField] private CharacterClass characterClass;
    [SerializeField] private CardReactionSequence actionSequence;

    public string Name => string.IsNullOrWhiteSpace(displayName) 
        ? name.SkipThroughFirstDash().SkipThroughFirstUnderscore().WithSpaceBetweenWords() 
        : displayName;
    public Sprite Art => art;
    public string Description => description;
    public string TypeDescription => "Reaction";
    public ResourceCost Cost => cost;
    public ResourceCost Gain => onPlayGain;
    public CardReactionSequence ActionSequence => actionSequence;
    public HashSet<CardTag> Tags => new HashSet<CardTag>();
    public Maybe<CharacterClass> LimitedToClass => new Maybe<CharacterClass>(characterClass);
    public CardActionSequence[] ActionSequences => new[] { CardActionSequence.ForReaction(ActionSequence.CardActions) };
    public Maybe<CardTypeData> ChainedCard => Maybe<CardTypeData>.Missing();
    public Rarity Rarity => Rarity.Starter;

    public ReactionCardType Initialized(ResourceCost cost, ResourceCost gain, CardReactionSequence action)
    {
        this.cost = cost;
        this.onPlayGain = gain;
        actionSequence = action;
        return this;
    }
}
