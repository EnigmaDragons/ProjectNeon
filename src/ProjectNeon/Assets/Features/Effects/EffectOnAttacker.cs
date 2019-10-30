using UnityEngine;
using System.Collections;

/**
 * Effects that applies to the attacker on an attack event
 */
public class EffectOnAttacker : Effect
{
    private Effect effect;
    private Member performer;
    private Target effectTarget;

    public EffectOnAttacker(Effect damage)
    {
        this.effect = damage;
        BattleEvent.Subscribe<Attack>(attack => Execute(attack), this);
    }

    void Effect.Apply(Member source, Target target)
    {
        this.performer = source;
        this.effectTarget = target;
    }

    void Execute(Attack attack)
    {
        this.effect.Apply(this.performer, new MemberAsTarget(attack.Attacker));
    }

}
