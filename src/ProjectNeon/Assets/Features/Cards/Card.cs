using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public sealed class Card
{
    [SerializeField] private int id;
    [SerializeField] private CardType type;

    public bool UseAsBasic;
    
    public CardType Type => UseAsBasic && LimitedToClass.IsPresent 
        ? LimitedToClass.Value.BasicCard 
        : type;
    
    public int Id => id;
    public string Name => type.Name;
    public ResourceCost Cost => type.Cost;
    public ResourceCost Gain => type.Gain;
    public Sprite Art => type.Art;
    public string Description => type.Description;
    public string TypeDescription => type.TypeDescription;
    public Maybe<CharacterClass> LimitedToClass => type.LimitedToClass;
    public CardActionSequence[] ActionSequences => type.ActionSequences;

    public Card(int id, CardType type)
    {
        this.id = id;
        this.type = type;
    }

    public void Play(Member source, Target[] targets, int amountPaid) => type.Play(source, targets, amountPaid);
    
    public static implicit operator CardType(Card card) => card.type;
}
