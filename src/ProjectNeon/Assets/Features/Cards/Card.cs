using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public sealed class Card
{
    [SerializeField] private int id;
    [SerializeField] private CardType type;
    [SerializeField] private Member owner;

    public bool UseAsBasic;
    
    public Maybe<CharacterClass> LimitedToClass => type.LimitedToClass;
    public CardType Type => UseAsBasic && LimitedToClass.IsPresent 
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

    public Card(int id, Member owner, CardType type)
    {
        this.owner = owner;
        this.id = id;
        this.type = type;
    }

    public void Play(Target[] targets, int amountPaid) => Type.Play(owner, targets, amountPaid);
    public void Play(Member source, Target[] targets, int amountPaid) => Type.Play(source, targets, amountPaid);
}
