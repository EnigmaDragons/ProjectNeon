
public sealed class Heal : Effect
{
    private readonly int _amount;

    public Heal(int amount) => _amount = amount;

    public void Apply(Member source, Target target) => target.ApplyToAllConscious(m => m.GainHp(_amount));

    public void Apply(Member source, Member target) => Apply(source, new MemberAsTarget(target));
}
