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
                 * @todo #454:15min Remove all attack.Target.Members[0] usages from code.
                 */
                if (target.Equals(attack.Target.Members[0]))
                {
                    _effect.Apply(_performer, new MemberAsTarget(attack.Attacker));
                }
                    
            }
        );
    }

}
