public sealed class Heal : Effect
{
    private readonly int _baseAmount;
    private readonly float _magicFactor;
    private readonly StatType _statType;

    public Heal(int baseAmount, float magicFactor, StatType statType)
    {
        _baseAmount = baseAmount;
        _magicFactor = magicFactor;
        _statType = statType;
    }
    
    public void Apply(EffectContext ctx) 
        => ctx.Target.ApplyToAllConscious(
            m => m.GainHp(_baseAmount + _magicFactor * ctx.Source.State[_statType]));
}
