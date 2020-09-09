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

    public Card(int id, Member owner, CardTypeData type)
    {
        this.owner = owner;
        this.id = id;
        this.type = type;
    }

    public void Play(Target[] targets, int amountPaid)
    {
        if (ActionSequences.Length > targets.Length)
            Log.Error($"{Name}: For {ActionSequences.Length} there are only {targets.Length} targets");

        for (var i = 0; i < ActionSequences.Length; i++)
        {
            var seq = ActionSequences[i];
            SequenceMessage.Queue(seq.cardActions.Play(Owner, targets[i], seq.Group, seq.Scope, amountPaid));
        }
    }

    public Card RevertedToStandard()
    {
        UseAsBasic = false;
        return this;
    }
}
