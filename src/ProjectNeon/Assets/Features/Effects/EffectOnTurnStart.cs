using UnityEngine;
using System.Collections;

/**
 * Effects that trigger on Turn Start.
 */
public class EffectOnTurnStart : Effect
{
    private Effect _effect;
    private Member _performer;
    private Target _effectTarget;

    public EffectOnTurnStart(Effect origin)
    {
        _effect = origin;
    }

    public void Apply(Member source, Target target)
    {
        _performer = source;
        _effectTarget = target;
        BattleEvent.Subscribe<TurnStart>((turnStart) => Execute(), this);
    }

    void Execute()
    {
        Debug.Log("Turn started!");
        _effect.Apply(_performer, _effectTarget);
    }
}
