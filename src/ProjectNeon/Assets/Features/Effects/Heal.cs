public sealed class Heal : Effect
{
    private readonly int _baseAmount;
    private readonly float _magicFactor;

    public Heal(int baseAmount, float magicFactor)
    {
        _baseAmount = baseAmount;
        _magicFactor = magicFactor;
    }
    
    public void Apply(EffectContext ctx) 
        => ctx.Target.ApplyToAllConscious(
            m => m.GainHp(_baseAmount + _magicFactor * ctx.Source.Magic()));
}
