
public class Evade : Effect
{
    private Member _performer;
    private Target _effectTarget;

    public void Apply(Member source, Target target)
    {
        _performer = source;
        _effectTarget = target;
        BattleEvent.Subscribe<AttackToPerform>(attackToPerform => Execute(attackToPerform.Attack), this);
    }

    void Execute(Attack attack)
    {
        _effectTarget.Members.ForEach(
            target => {
                if (target.Equals(attack.Target.Members[0]))
                {
                    attack.Effect = new NoEffect();
                }
            }
        );
    }
}