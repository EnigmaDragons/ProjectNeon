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
    public int IntAmount => FloatAmount.Value.CeilingInt();
    public float TotalAmount => FloatAmount.Value + BaseAmount;
    public int TotalIntAmount => TotalAmount.CeilingInt();
    
    public IntReference NumberOfTurns = new IntReference(0);
    public StringReference EffectScope = new StringReference { UseConstant = false };
    public bool HitsRandomTargetMember;
    
    public StatusDetail StatusDetail => new StatusDetail(StatusTag, string.IsNullOrWhiteSpace(StatusDetailText) ? Maybe<string>.Missing() : StatusDetailText);
    public StatusTag StatusTag;
    public string StatusDetailText;
    
    public int TurnDelay;
    [TextArea(minLines:1, maxLines:9)] public string Formula = "";
    public StringReference FlavorText = new StringReference();
    public CardActionsData ReferencedSequence;
    
    public bool IsReactionCard => ReactionSequence != null;
    public bool IsReactionEffect => ReactionEffect != null;
    public ReactionConditionType ReactionConditionType;
    [FormerlySerializedAs("ReactionCard")] public ReactionCardType ReactionSequence;
    public CardReactionSequence ReactionEffect;

    public CardType BonusCardType;
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
