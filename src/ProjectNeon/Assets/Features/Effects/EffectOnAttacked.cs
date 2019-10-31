using UnityEngine;
using System.Collections;

/**
 * Effects that trigger to the target of an attack upon an attack event
 */
public class EffectOnAttacked : Effect
{
    private Effect _effect;
    private Member _performer;
    private Target _effectTarget;

    public EffectOnAttacked(Effect shield)
    {
        _effect = shield;
        BattleEvent.Subscribe<Attack>(attack => Execute(attack), this);
    }

    void Effect.Apply(Member source, Target target)
    {
        _performer = source;
        _effectTarget = target;
    }

    void Execute(Attack attack)
    {
        _effectTarget.Members.ForEach(
            target => {
                    if (target.Equals(attack.Target))
                        _effect.Apply(_performer, _effectTarget);
                }
        );
    }
}
