using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public sealed class EffectData
{
    public static EffectData Nothing => new EffectData(); 
    
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
    public bool ApplyToEachMemberIndividually = false;
    
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
    private static EffectData Clone(EffectData e)
        => new EffectData
        {
            EffectType = e.EffectType,
            Conditions = e.Conditions,
            BaseAmount = e.BaseAmount,
            FloatAmount = e.FloatAmount,
            
            DurationFormula = e.DurationFormula,
            EffectScope = e.EffectScope,
            HitsRandomTargetMember = e.HitsRandomTargetMember,
            TargetsSource = e.TargetsSource,
            ReTargetScope = e.ReTargetScope,
            ApplyToEachMemberIndividually = e.ApplyToEachMemberIndividually,
            
            StatusTag = e.StatusTag,
            StatusDetailText = e.StatusDetailText,
            
            TurnDelay = e.TurnDelay,
            
            Formula = e.Formula,
            InterpolatePartialFormula = e.InterpolatePartialFormula,
            FlavorText = e.FlavorText,
            ReferencedSequence = e.ReferencedSequence,
            
            ReactionConditionType = e.ReactionConditionType,
            ReactionEffectScope = e.ReactionEffectScope,
            ReactionSequence = e.ReactionSequence,
            ReactionEffect = e.ReactionEffect,
            
            BonusCardType = e.BonusCardType
        };

    public static EffectData Immediately(this EffectData e)
    {
        var newData = Clone(e);
        newData.TurnDelay = 0;
        return newData;
    }
    
    public static EffectData WithoutAutoRetargeting(this EffectData e)
    {
        var newData = Clone(e);
        newData.ReTargetScope = AutoReTargetScope.None;
        newData.TargetsSource = false;
        return newData;
    }

    public static EffectCondition Condition(this EffectData e) =>
        e.Conditions != null 
        && e.Conditions.Length > 0 
            ? (EffectCondition)new AndEffectCondition(e.Conditions.Cast<EffectCondition>().ToArray()) 
            : new NoEffectCondition();

    public static InterpolateFriendlyFormula InterpolateFriendlyFormula(this EffectData e)
        => new InterpolateFriendlyFormula
        {
            FullFormula = e.Formula,
            InterpolatePartialFormula = e.InterpolatePartialFormula
        };
}
