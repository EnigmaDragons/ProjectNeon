using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class InterceptAttack : Effect
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
            target => {
                if (target.Equals(attack.Target))
                {
                    attack.Target = _performer;
                }
            }
        );
    }
}

