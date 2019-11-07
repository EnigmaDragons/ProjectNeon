
using UnityEngine;

public class InterceptAttack : Effect
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
        if (_effectTarget.Equals(attack.Target))
        {
            attack.Effect = new NoEffect();
            new Attack(attack.Multiplier).Apply(attack.Attacker, _performer);
        };
    }
}

