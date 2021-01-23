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
    
    public StatusDetail StatusDetail => new StatusDetail(StatusTag, string.IsNullOrWhiteSpace(StatusDetailText) ? Maybe<string>.Missing() : StatusDetailText);
    public StatusTag StatusTag;
    public string StatusDetailText;
    
    public int TurnDelay;
    public string Formula = "";
    public StringReference FlavorText = new StringReference();
    public CardActionsData ReferencedSequence;
    public bool IsReactionCard => ReactionCard != null;

    public ReactionConditionType ReactionConditionType;
    [FormerlySerializedAs("ReactionSequence")] public ReactionCardType ReactionCard;
    public CardReactionSequence ReactionEffect;
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
            ReactionCard = e.ReactionCard,
            StatusTag = e.StatusTag,
            TurnDelay = 0,
            Formula = e.Formula,
            FlavorText = e.FlavorText
        };
}
