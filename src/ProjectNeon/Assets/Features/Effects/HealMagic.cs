
public class HealMagic : Effect
{
    private readonly float _multiplier;

    public HealMagic(float multiplier) 
        => _multiplier = multiplier;

    public void Apply(Member source, Target target) 
        => target.ApplyToAllConscious(m => m.GainHp(_multiplier * source.State.Magic()));
}
