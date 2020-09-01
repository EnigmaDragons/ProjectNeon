
public sealed class Heal : Effect
{
    private readonly int _amount;

    public Heal(int amount) => _amount = amount;
    
    public void Apply(EffectContext ctx) => ctx.Target.ApplyToAllConscious(m => m.GainHp(_amount));
}
