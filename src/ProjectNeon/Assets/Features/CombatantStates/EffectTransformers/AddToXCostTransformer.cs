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
            m.AddEffectTransformer(new AddToXCostTransformer(_data)));
    }
}

public class AddToXCostTransformer : EffectTransformerBase
{
    public AddToXCostTransformer(EffectData data) : base(false, data.NumberOfTurns, data.IntAmount, data.StatusDetail, 
        (effect, context) => effect.Formula.Contains("X"),
        (effect, context) => new EffectData
        {
            EffectType = effect.EffectType,
            BaseAmount = effect.BaseAmount,
            FloatAmount = effect.FloatAmount,
            NumberOfTurns = effect.NumberOfTurns,
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