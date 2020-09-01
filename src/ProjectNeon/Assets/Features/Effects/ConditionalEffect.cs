
public abstract class ConditionalEffect : Effect
{
    protected Effect Effect;
    protected Member Source;
    protected Target Target;

    public ConditionalEffect(Effect effect)
    {
        Effect = effect;
    }

    public void Apply(EffectContext ctx)
    {
        Source = ctx.Source;
        Target = ctx.Target;
        if (Condition())
        {
            Effect.Apply(ctx);
        }

    }

    public abstract bool Condition();
}