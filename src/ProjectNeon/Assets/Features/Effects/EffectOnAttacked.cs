using UnityEngine;
using System.Collections;
using System.Linq;

/**
 * Effects that trigger to the target of an attack upon an attack event
 */
public class EffectOnAttacked : Effect
{
    private CardActionsData _effect;
    private int _duration;
    private Member _performer;
    private Target _effectTarget;

    public EffectOnAttacked(CardActionsData effect, int duration)
    {
        _effect = effect;
        _duration = duration;
    }

    void Effect.Apply(Member source, Target target)
    {
        _performer = source;
        _effectTarget = target;
        _performer.State.AddReactiveState(new ReactiveState(state => Message.Subscribe<Finished<Attack>>(x => Execute(x.Message), state), _duration, false));
    }

    void Execute(Attack attack)
    {
        if (_effectTarget.Members.Any(x => attack.Target.Members.Any(t => t.Name == x.Name)))
            SequenceMessage.Queue(_effect.Play(_performer, new Single(attack.Attacker), Group.Opponent, Scope.One, 0));
    }
}
