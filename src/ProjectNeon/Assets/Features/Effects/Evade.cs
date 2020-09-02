
public class Evade : Effect
{
    private int _number;

    public Evade(int number) => _number = number;

    public void Apply(EffectContext ctx)
    {
        ctx.Target.ApplyToAllConscious(x => x.AdjustEvade(_number));
    }
}