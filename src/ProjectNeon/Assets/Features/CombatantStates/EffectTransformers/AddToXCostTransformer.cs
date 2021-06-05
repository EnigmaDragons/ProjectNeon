using UnityEngine;

public sealed class EffectAddToXCostTransformer : Effect
{
    private readonly EffectData _data;

    public EffectAddToXCostTransformer(EffectData data)
    {
        _data = data;
    }

    public void Apply(EffectContext ctx)
    {
        ctx.Target.ApplyToAllConscious(m =>
            m.AddEffectTransformer(new AddToXCostTransformer(ctx.Source.Id, _data, Mathf.CeilToInt(Formula.Evaluate(ctx.SourceSnapshot.State, m, _data.DurationFormula, ctx.XPaidAmount)))));
    }
}

public class AddToXCostTransformer : EffectTransformerBase
{
    public AddToXCostTransformer(int originatorId, EffectData data, int numberOfTurns) : base(originatorId, false, numberOfTurns, data.IntAmount, data.StatusDetail, 
        (effect, context) => effect.Formula.Contains("X") || effect.DurationFormula.Contains("X"),
        (effect, context) => new EffectData
        {
            EffectType = effect.EffectType,
            BaseAmount = effect.BaseAmount,
            FloatAmount = effect.FloatAmount,
            DurationFormula = effect.DurationFormula.Replace("X", "(X + 1)"),
            EffectScope = effect.EffectScope,
            HitsRandomTargetMember = effect.HitsRandomTargetMember,
            ReferencedSequence = effect.ReferencedSequence,
            ReactionSequence = effect.ReactionSequence,
            StatusTag = effect.StatusTag,
            TurnDelay = effect.TurnDelay,
            Formula = effect.Formula.Replace("X", "(X + 1)"),
            FlavorText = effect.FlavorText
        }) {}
}