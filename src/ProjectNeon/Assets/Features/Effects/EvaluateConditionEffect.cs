using System.Linq;

public class EvaluateConditionEffect : Effect
{
    private readonly string _conditionName;

    public EvaluateConditionEffect(EffectData e)
        => _conditionName = string.IsNullOrWhiteSpace(e.EffectScope) ? e.Conditions.First().name : e.EffectScope;

    public void Apply(EffectContext ctx)
    {
        ctx.ScopedData.RecordTrueCondition(_conditionName);
    }
}