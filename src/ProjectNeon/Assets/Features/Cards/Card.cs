using System;
using UnityEngine;

[Serializable]
public sealed class Card
{
    [SerializeField] private int id;
    [SerializeField] private CardTypeData type;
    [SerializeField] private Member owner;

    public bool UseAsBasic;

    public CardTypeData BasicType => LimitedToClass.Value.BasicCard;
    public Maybe<CharacterClass> LimitedToClass => type.LimitedToClass;
    public CardTypeData Type => UseAsBasic && LimitedToClass.IsPresent 
        ? LimitedToClass.Value.BasicCard 
        : type;

    public Member Owner => owner;
    
    public int Id => id;
    public string Name => Type.Name;
    public ResourceCost Cost => Type.Cost;
    public ResourceCost Gain => Type.Gain;
    public Sprite Art => Type.Art;
    public string Description => Type.Description;
    public string TypeDescription => Type.TypeDescription;
    public CardActionSequence[] ActionSequences => Type.ActionSequences;
    public Maybe<CardTypeData> ChainedCard => Type.ChainedCard;
    public bool Hasty => type.Is(CardTag.Haste);
    public Maybe<ResourceQuantity> LockedXValue { get; private set; } = Maybe<ResourceQuantity>.Missing();

    public Card(int id, Member owner, CardTypeData type)
    {
        this.owner = owner;
        this.id = id;
        this.type = type;
    }
    
    public Card RevertedToStandard()
    {
        UseAsBasic = false;
        ClearXValue();
        return this;
    }

    public void SetXValue(ResourceQuantity r) => LockedXValue = new Maybe<ResourceQuantity>(r);
    public void ClearXValue() => LockedXValue = Maybe<ResourceQuantity>.Missing();
}
