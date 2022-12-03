public class EvaluateConditionEffect : Effect
{
    private readonly string _conditionName;

    public EvaluateConditionEffect(string conditionName)
        => _conditionName = conditionName;

    public void Apply(EffectContext ctx)
    {
        ctx.ScopedData.RecordTrueCondition(_conditionName);
    }
}