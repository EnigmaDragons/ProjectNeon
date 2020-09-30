using System;
using UnityEngine;

[Serializable]
public sealed class EffectData
{
    public static readonly EffectData Nothing = new EffectData(); 
    
    public EffectType EffectType;
    public FloatReference FloatAmount = new FloatReference();
    public IntReference NumberOfTurns = new IntReference();
    public StringReference EffectScope = new StringReference();
    public bool HitsRandomTargetMember;
    public int IntAmount => Mathf.CeilToInt(FloatAmount.Value);
    public CardActionsData ReferencedSequence;
    public ReactionCardType ReactionSequence;
    public StatusTag StatusTag;
    public bool AtStartOfNextTurn;
    [Obsolete] public EffectData origin = Nothing; //obsolete, but can't delete yet for data loss reasons
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
