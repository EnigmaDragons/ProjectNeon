
public class HealMagic : Effect
{
    private readonly float _multiplier;

    public HealMagic(float multiplier) 
        => _multiplier = multiplier;

    public void Apply(EffectContext ctx)
        => ctx.Target.ApplyToAllConscious(m => m.GainHp(_multiplier * ctx.Source.State.Magic()));
}
