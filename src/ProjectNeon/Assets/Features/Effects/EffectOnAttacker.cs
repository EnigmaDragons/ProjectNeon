using UnityEngine;
using System.Collections;

/**
 * Effects that applies to the attacker on an attack event.
 * 
 */
public class EffectOnAttacker : Effect
{
    private Effect _effect;
    private Member _performer;
    private Target _effectTarget;

    public EffectOnAttacker(Effect damage)
    {
        _effect = damage;
    }

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
                /**
                 * @todo #454:30min Refactor Target - Member relation. At the moment the responsible for discovering target
                 *  logic and using it in the effect is the effect itself, like below, in attack.Target.Members[0]. This code
                 *  should be polymorphically refactored so each Target implemention knows if it has one or more Members, and 
                 *  treat it accordingly. After this, remove all (attack.Target.Members[0]) references from code.
                 */
                if (target.Equals(attack.Target.Members[0]))
                {
                    _effect.Apply(_performer, new MemberAsTarget(attack.Attacker));
                }
                    
            }
        );
    }

}
