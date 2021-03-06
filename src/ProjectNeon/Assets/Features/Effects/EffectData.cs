using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public sealed class EffectData
{
    public static readonly EffectData Nothing = new EffectData(); 
    
    public EffectType EffectType;
    public StaticEffectCondition[] Conditions;
    public IntReference BaseAmount = new IntReference(0);
    public FloatReference FloatAmount = new FloatReference(0);
    public int IntAmount => FloatAmount.Value.CeilingInt();
    public float TotalAmount => FloatAmount.Value + BaseAmount;
    public int TotalIntAmount => TotalAmount.CeilingInt();
    
    public string DurationFormula = "0";
    public StringReference EffectScope = new StringReference { UseConstant = false };
    public bool HitsRandomTargetMember;
    public bool TargetsSource;
    public AutoReTargetScope ReTargetScope = AutoReTargetScope.None;
    
    public StatusDetail StatusDetail => new StatusDetail(StatusTag, string.IsNullOrWhiteSpace(StatusDetailText) ? Maybe<string>.Missing() : StatusDetailText);
    public StatusTag StatusTag;
    public string StatusDetailText;
    
    public int TurnDelay;
    
    [TextArea(minLines:1, maxLines:9)] public string Formula = "";
    public InterpolatePartialFormula InterpolatePartialFormula = new InterpolatePartialFormula();
    public StringReference FlavorText = new StringReference();
    public CardActionsData ReferencedSequence;
    
    public bool IsReactionCard => ReactionSequence != null;
    public bool IsReactionEffect => ReactionEffect != null;
    public ReactionConditionType ReactionConditionType;
    public StringReference ReactionEffectScope = new StringReference();
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
            BaseAmount = e.BaseAmount,
            DurationFormula = e.DurationFormula,
            EffectScope = e.EffectScope,
            HitsRandomTargetMember = e.HitsRandomTargetMember,
            ReferencedSequence = e.ReferencedSequence,
            ReactionSequence = e.ReactionSequence,
            StatusTag = e.StatusTag,
            TurnDelay = 0,
            Formula = e.Formula,
            FlavorText = e.FlavorText
        };

    public static EffectCondition Condition(this EffectData e) =>
        e.Conditions != null && e.Conditions.Length > 0 
            ? (EffectCondition)new AndEffectCondition(e.Conditions.Cast<EffectCondition>().ToArray()) 
            : new NoEffectCondition();

    public static InterpolateFriendlyFormula InterpolateFriendlyFormula(this EffectData e)
        => new InterpolateFriendlyFormula
        {
            FullFormula = e.Formula,
            InterpolatePartialFormula = e.InterpolatePartialFormula
        };
}
