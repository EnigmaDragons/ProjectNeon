using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public sealed class Card : CardTypeData
{
    [SerializeField] private int id;
    [SerializeField] private CardTypeData type;
    [SerializeField] private Member owner;
    [SerializeField] private readonly Maybe<Color> tint;
    [SerializeField] private readonly Maybe<Sprite> ownerBust;

    private readonly List<ITemporalCardState> _temporalStates = new List<ITemporalCardState>();
    
    public CardMode Mode { get; private set; }
    public bool IsActive => Mode != CardMode.Dead && Mode != CardMode.Glitched;

    public bool IsBasic => Owner != null && (Mode == CardMode.Basic || Owner.BasicCard.IsPresentAnd(b => b.Id == type.Id));
    private CardTypeData _type => IsBasic ? Owner.BasicCard.Value : type;
    public CardTypeData Type => IsBasic ? Owner.BasicCard.Value : this;
    public CardTypeData BaseType => type;
    
    public Member Owner => owner;

    public int CardId => id;
    public int Id => _type.Id;
    public string Name => _type?.Name ?? "Card Not Initialized";
    public string NameTerm => _type?.NameTerm ?? "Card Not Initialized";
    public string DescriptionTerm => _type?.DescriptionTerm ?? "Card Not Initialized";
    public IResourceAmount Cost => new InMemoryResourceAmount(
        Math.Max(_type.Cost.BaseAmount + _temporalStates.Where(x => x.IsActive).Sum(x => x.CostAdjustment), 0), _type.Cost.ResourceType, _type.Cost.PlusXCost);
    public CardSpeed Speed => _type.Speed;
    public Sprite Art => _type.Art;
    public string Description => _type.Description;
    public CardDescriptionV2 DescriptionV2 => _type.DescriptionV2;
    public HashSet<CardTag> Tags => _type.Tags;
    public string TypeDescription => _type.TypeDescription;
    public CardActionSequence[] ActionSequences => _type.ActionSequences ?? new CardActionSequence[0];
    public Maybe<CardTypeData> ChainedCard => _type.ChainedCard;
    public Maybe<CardTypeData> SwappedCard => _type.SwappedCard;
    public Rarity Rarity => _type.Rarity;
    public Maybe<ResourceQuantity> LockedXValue { get; private set; } = Maybe<ResourceQuantity>.Missing();
    public Maybe<Color> OwnerTint => tint;
    public Maybe<Sprite> OwnerBust => ownerBust;
    public HashSet<string> Archetypes => _type.Archetypes;
    public bool IsAttack => _type.IsAttack();
    public Maybe<CardCondition> HighlightCondition => _type.HighlightCondition;
    public Maybe<CardCondition> UnhighlightCondition => _type.UnhighlightCondition;
    public Maybe<TargetedCardCondition> TargetedHighlightCondition => _type.TargetedHighlightCondition;
    public Maybe<TargetedCardCondition> TargetedUnhighlightCondition => _type.TargetedUnhighlightCondition;
    public bool IsSinglePlay => _temporalStates.Any(x => x.IsSinglePlay) || _type.IsSinglePlay;
    public bool IsQuick => Speed == CardSpeed.Quick;

    public Card(int id, Member owner, CardTypeData type)
        : this(id, owner, type, Maybe<Color>.Missing(), Maybe<Sprite>.Missing()) {}
    
    public Card(int id, Member owner, CardTypeData type, Maybe<Color> tint, Maybe<Sprite> ownerBust)
    {
        this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        this.id = id;
        this.type = type ?? throw new ArgumentNullException(nameof(CardTypeData));
        this.tint = tint;
        this.ownerBust = ownerBust;
    }

    public Card RevertedToStandard()
    {
        if (Mode != CardMode.Normal)
            DevLog.Info($"Reverted {Name} {CardId} To {Mode}");
        Mode = CardMode.Normal;
        ClearXValue();
        return this;
    }

    public void SetXValue(ResourceQuantity r) => LockedXValue = new Maybe<ResourceQuantity>(r);
    public void ClearXValue() => LockedXValue = Maybe<ResourceQuantity>.Missing();
    
    public void TransitionTo(CardMode mode)
    {
        // Very, very hard to understand
        if (Mode == CardMode.Normal || Mode == CardMode.Basic)
            Mode = mode;
        else if (Mode == CardMode.Dead && (mode == CardMode.Normal || mode == CardMode.Glitched))
            Mode = mode;
        DevLog.Info($"Transitioned {Name} {CardId} To {Mode}");
    }
    
    public void AddState(ITemporalCardState state) => _temporalStates.Add(state);
    public void OnTurnStart() => _temporalStates.Where(x => x.IsActive).ToArray().ForEach(x => x.OnTurnStart());
    public void OnTurnEnd() => _temporalStates.Where(x => x.IsActive).ToArray().ForEach(x => x.OnTurnEnd());
    public void OnPlayCard() => _temporalStates.Where(x => x.IsActive).ToArray().ForEach(x => x.OnCardPlay());
    public void CleanExpiredStates() => _temporalStates.RemoveAll(x => !x.IsActive);
}

public static class CardRuleExtensions
{
    public static bool IsAttack(this CardTypeData c) 
        => !c.TypeDescription.Equals("Spell")
           && !c.TargetsOnlyFriendlies()
           && (c.Tags.Contains(CardTag.Attack) 
                || c.TypeDescription.Equals("Attack") 
                || (c.Description.Contains("Deal") && !c.Tags.Contains(CardTag.DamageOverTime)));

    public static bool TargetsOnlyFriendlies(this CardTypeData c)
        => c.ActionSequences.AllNonAlloc(a => a.Group == Group.Self || a.Group == Group.Ally);
}
