﻿
using UnityEngine;

class InterceptAttack : Effect
{
    private Member _performer;
    private Target _effectTarget;

    void Effect.Apply(Member source, Target target)
    {
        _performer = source;
        _effectTarget = target;
        BattleEvent.Subscribe<AttackPerformed>(attackPerformed => Execute(attackPerformed.Attack), this);
    }

    void Execute(Attack attack)
    {
        _effectTarget.Members.ForEach(
            target => {
                if (target.Equals(attack.Target.Members[0]))
                {
                    new Attack(attack.Damage).Apply(attack.Attacker, _performer);
                }
            }
        );
    }
}

