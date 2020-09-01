
public class InterceptAttack : Effect
{
    private Member _performer;
    private Target _effectTarget;
    private int _duration;

    public InterceptAttack(int duration)
        => _duration = duration;

    public void Apply(EffectContext ctx)
    {
        _performer = ctx.Source;
        _effectTarget = ctx.Target;
        // TODO: Implement
    }

    void Execute(Attack attack, Member member)
    {
        if (attack.Target.Members.Length == 1 && attack.Target.Members[0].Name == member.Name)
            attack.Target = new Single(_performer);
    }
}
