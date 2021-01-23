using System;
using Features.CombatantStates.Reactions;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public sealed class EffectData
{
    public static readonly EffectData Nothing = new EffectData(); 
    
    public EffectType EffectType;
    public IntReference BaseAmount = new IntReference(0);
    public FloatReference FloatAmount = new FloatReference(0);
    public int IntAmount => Mathf.CeilToInt(FloatAmount.Value);
    public IntReference NumberOfTurns = new IntReference(0);
    public StringReference EffectScope = new StringReference { UseConstant = false };
    public bool HitsRandomTargetMember;
    public StatusTag StatusTag;
    public int TurnDelay;
    public string Formula = "";
    public StringReference FlavorText = new StringReference();
    public CardActionsData ReferencedSequence;
    public bool IsReactionCard => ReactionSequence != null;
    public ReactionConditionType ReactionConditionType;
    public ReactionCardType ReactionSequence; // Name is wrong. Hard to change currently. Should be named ReactionCard
    public CardReactionSequence ReactionEffect; // Cannot use yet. Editor is broken for this. I can't figure out why
}

public static class EffectDataExtensions
{
    public static EffectData Immediately(this EffectData e)
        => new EffectData
        {
            EffectType = e.EffectType,
            FloatAmount = e.FloatAmount,
            NumberOfTurns = e.NumberOfTurns,
            EffectScope = e.EffectScope,
            HitsRandomTargetMember = e.HitsRandomTargetMember,
            ReferencedSequence = e.ReferencedSequence,
            ReactionSequence = e.ReactionSequence,
            StatusTag = e.StatusTag,
            TurnDelay = 0,
            Formula = e.Formula,
            FlavorText = e.FlavorText
        };
}
