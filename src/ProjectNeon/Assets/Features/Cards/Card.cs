using System;
using UnityEngine;

[Serializable]
public sealed class Card
{
    [SerializeField] private int id;
    [SerializeField] private CardTypeData type;
    [SerializeField] private Member owner;

    public CardMode Mode { get; private set; }
    public bool IsActive => Mode != CardMode.Dead && Mode != CardMode.Glitched;
    
    public CardTypeData Type => Mode == CardMode.Basic && Owner != null
        ? Owner.BasicCard.Value
        : type;

    public Member Owner => owner;
    
    public int Id => id;
    public string Name => Type.Name;
    public IResourceAmount Cost => Type.Cost;
    public IResourceAmount Gain => Type.Gain;
    public Sprite Art => Type.Art;
    public string Description => Type.Description;
    public string TypeDescription => Type.TypeDescription;
    public CardActionSequence[] ActionSequences => Type.ActionSequences;
    public Maybe<CardTypeData> ChainedCard => Type.ChainedCard;
    public Maybe<ResourceQuantity> LockedXValue { get; private set; } = Maybe<ResourceQuantity>.Missing();

    public Card(int id, Member owner, CardTypeData type)
    {
        this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        this.id = id;
        this.type = type;
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
}
