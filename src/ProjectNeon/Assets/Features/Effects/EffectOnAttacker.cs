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
        //implement
    }

    void Execute(Attack attack)
    {
        if (_effectTarget.Equals(attack.Target))
            _effect.Apply(_performer, new Single(attack.Attacker));

    }

}
