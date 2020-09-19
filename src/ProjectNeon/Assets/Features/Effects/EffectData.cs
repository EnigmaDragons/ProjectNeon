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
    [Obsolete] public EffectData origin = Nothing; //obsolete, but can't delete yet for data loss reasons
}
