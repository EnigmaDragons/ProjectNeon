using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public sealed class Card
{
    [SerializeField] private int id;
    [SerializeField] private CardTypeData type;
    [SerializeField] private Member owner;
    [SerializeField] private readonly Maybe<Color> tint;

    private readonly List<ITemporalCardState> _temporalStates = new List<ITemporalCardState>();
    
    public CardMode Mode { get; private set; }
    public bool IsActive => Mode != CardMode.Dead && Mode != CardMode.Glitched;
    
    public CardTypeData Type => Mode == CardMode.Basic && Owner != null
        ? Owner.BasicCard.Value
        : type;

    public Member Owner => owner;
    
    public int Id => id;
    public string Name => Type.Name;
    public IResourceAmount Cost => new InMemoryResourceAmount(Math.Max(Type.Cost.BaseAmount + _temporalStates.Where(x => x.IsActive).Sum(x => x.CostAdjustment), 0), Type.Cost.ResourceType.Name, Type.Cost.PlusXCost);
    public IResourceAmount Gain => Type.Gain;
    public Sprite Art => Type.Art;
    public string Description => Type.Description;
    public string TypeDescription => Type.TypeDescription;
    public CardActionSequence[] ActionSequences => Type.ActionSequences;
    public Maybe<CardTypeData> ChainedCard => Type.ChainedCard;
    public Maybe<ResourceQuantity> LockedXValue { get; private set; } = Maybe<ResourceQuantity>.Missing();
    public Color Tint => tint.OrDefault(Color.white);
    public HashSet<string> Archetypes => Type.Archetypes;
    public bool IsAttack => Type.Tags.Contains(CardTag.Attack) || Type.TypeDescription.Equals("Attack");
    
    public Card(int id, Member owner, CardTypeData type)
        : this(id, owner, type, Maybe<Color>.Missing()) {}
    
    public Card(int id, Member owner, CardTypeData type, Maybe<Color> tint)
    {
        this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        this.id = id;
        this.type = type;
        this.tint = tint;
    }
    
    public Card RevertedToStandard()
    {
        TransitionTo(CardMode.Normal);
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
    }
    
    public void AddState(ITemporalCardState state) => _temporalStates.Add(state);
    public void OnTurnStart() => _temporalStates.Where(x => x.IsActive).ToArray().ForEach(x => x.OnTurnStart());
    public void OnTurnEnd() => _temporalStates.Where(x => x.IsActive).ToArray().ForEach(x => x.OnTurnEnd());
    public void OnPlayCard() => _temporalStates.Where(x => x.IsActive).ToArray().ForEach(x => x.OnCardPlay());
    public void CleanExpiredStates() => _temporalStates.RemoveAll(x => !x.IsActive);
}
