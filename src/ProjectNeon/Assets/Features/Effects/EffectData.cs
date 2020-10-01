using System;
using UnityEngine;

[Serializable]
public sealed class EffectData
{
    public static readonly EffectData Nothing = new EffectData(); 
    
    public EffectType EffectType;
    public IntReference BaseAmount = new IntReference();
    public FloatReference FloatAmount = new FloatReference();
    public int IntAmount => Mathf.CeilToInt(FloatAmount.Value);
    public IntReference NumberOfTurns = new IntReference();
    public StringReference EffectScope = new StringReference();
    public bool HitsRandomTargetMember;
    public CardActionsData ReferencedSequence;
    public ReactionCardType ReactionSequence;
    public StatusTag StatusTag;
    public bool AtStartOfNextTurn;
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
            AtStartOfNextTurn = false
        };
}
