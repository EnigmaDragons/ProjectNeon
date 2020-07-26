using UnityEngine;
using System.Collections;

/**
 * Effects that trigger to the target of an attack upon an attack event
 */
public class EffectOnAttacked : Effect
{
    private CardActionsData _effect;
    private Member _performer;
    private Target _effectTarget;

    public EffectOnAttacked(CardActionsData effect)
    {
        _effect = effect;
    }

    void Effect.Apply(Member source, Target target)
    {
        _performer = source;
        _effectTarget = target;
        Message.Subscribe<AttackPerformed>(attackPerformed => Execute(attackPerformed.Attack), this);
    }

    void Execute(Attack attack)
    {
        if (_effectTarget.Equals(attack.Target))
            SequenceMessage.Queue(_effect.Play(_performer, new Single(attack.Attacker), Group.Opponent, Scope.One, 0));
    }
}
