using UnityEngine;
using System.Collections;

/**
 * Effects that trigger to the target of an attack upon an attack event
 */
public class EffectOnAttacked : Effect
{
    private Effect effect;
    private Member performer;
    private Target effectTarget;

    public EffectOnAttacked(Effect shield)
    {
        this.effect = shield;
        BattleEvent.Subscribe<Attack>(attack => Execute(attack), this);
    }

    void Effect.Apply(Member source, Target target)
    {
        this.performer = source;
        this.effectTarget = target;
    }

    void Execute(Attack attack)
    {
        this.effect.Apply(this.performer, this.effectTarget);
    }
}
