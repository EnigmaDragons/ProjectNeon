public class SetScopedVariableToRandom : Effect
{
    private readonly EffectData _e;

    public SetScopedVariableToRandom(EffectData e) => _e = e;

    public void Apply(EffectContext ctx)
    {
        var random = Rng.Int(1, _e.IntAmount + 1);
        foreach (var m in ctx.Target.Members.GetConscious())
            ctx.ScopedData.SetVariable(_e.EffectScope.Value, random);
        Message.Publish(new DieRolled { Number = random });
    }
}