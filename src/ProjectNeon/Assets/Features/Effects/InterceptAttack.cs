
public class InterceptAttack : Effect
{
    private Member _performer;
    private Target _effectTarget;
    private int _duration;

    public InterceptAttack(int duration)
        => _duration = duration;

    public void Apply(Member source, Target target)
    {
        _performer = source;
        _effectTarget = target;
        // TODO: Implement
    }

    void Execute(Attack attack, Member member)
    {
        if (attack.Target.Members.Length == 1 && attack.Target.Members[0].Name == member.Name)
            attack.Target = new Single(_performer);
    }
}
