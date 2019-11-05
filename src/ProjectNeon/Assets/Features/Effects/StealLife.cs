using UnityEngine;
using System.Collections;
using System;

/**
 * Heals the attacker based on the damage the attack inflicted.
 * 
 */
public class StealLife : Effect
{
    private Member _performer;
    private Target _effectTarget;
    private float _ratio;

    public StealLife() : this(1F)
    {
        
    }

    public StealLife(float ratio)
    {
        _ratio = ratio;
    }

    void Effect.Apply(Member source, Target target)
    {
        _performer = source;
        _effectTarget = target;
        BattleEvent.Subscribe<Attack>(attack => Execute(attack), this);
    }

    void Execute(Attack attack)
    {
        _effectTarget.Members.ForEach(
            target =>
            {
                if (target.Equals(attack.Attacker))
                {
                    new SimpleEffect(
                        m => m.GainHp(attack.Damage * _ratio)
                    ).Apply(_performer, new MemberAsTarget(target));
                }
            }
        );
    }

}
