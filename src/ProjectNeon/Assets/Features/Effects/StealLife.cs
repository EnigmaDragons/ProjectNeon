using UnityEngine;
using System.Collections;

/**
 * Heals the attacker based on the damage the attack inflicted.
 * 
 */
public class StealLife : Effect
{
    private Member _performer;
    private Target _effectTarget;

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
                        m => m.GainHp(attack.Damage)
                    ).Apply(_performer, new MemberAsTarget(target));
                }
            }
    );
    }

}
