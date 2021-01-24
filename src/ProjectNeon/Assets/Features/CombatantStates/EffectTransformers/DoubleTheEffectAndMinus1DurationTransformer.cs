using System;

public sealed class EffectDoubleTheEffectAndMinus1Duration : Effect
{
    private readonly EffectData _data;

    public EffectDoubleTheEffectAndMinus1Duration(EffectData data)
    {
        _data = data;
    }

    public void Apply(EffectContext ctx)
    {
        ctx.Target.ApplyToAllConscious(m =>
            m.AddEffectTransformer(new DoubleTheEffectAndMinus1DurationTransformer(_data)));
    }
}

public class DoubleTheEffectAndMinus1DurationTransformer : EffectTransformerBase
{
    public DoubleTheEffectAndMinus1DurationTransformer(EffectData data) : base(false, data.NumberOfTurns, data.IntAmount, data.StatusDetail, 
        (effect, context) => effect.NumberOfTurns > 0,
        (effect, context) =>
        {
            return new EffectData
            {
                EffectType = effect.EffectType,
                BaseAmount = new IntReference(effect.BaseAmount.Value * 2),
                FloatAmount = new FloatReference(effect.FloatAmount.Value * 2f),
                NumberOfTurns = new IntReference(effect.NumberOfTurns - 1),
                EffectScope = effect.EffectScope,
                HitsRandomTargetMember = effect.HitsRandomTargetMember,
                ReferencedSequence = effect.ReferencedSequence,
                ReactionSequence = effect.ReactionSequence,
                StatusTag = effect.StatusTag,
                TurnDelay = effect.TurnDelay,
                Formula = $"({effect.Formula}) * 2",
                FlavorText = effect.FlavorText
            };
        }) {}
}