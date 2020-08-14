using System.Collections.Generic;

/**
 * Heals the attacker based on the damage the attack inflicted.
 * 
 */
public class StealLife : Effect
{
    private Member _performer;
    private Target _effectTarget;
    private float _ratio;
    private int _duration;
    private Dictionary<Member, ReactiveState> _buffs = new Dictionary<Member, ReactiveState>();
    
    public StealLife(float ratio, int duration)
    {
        _ratio = ratio;
        _duration = duration;
    }

    void Effect.Apply(Member source, Target target)
    {
        _performer = source;
        _effectTarget = target;
        foreach (var member in target.Members)
        {
            // TODO: Implement
            //var state = new ReactiveState(x => Message.Subscribe<Finished<Attack>>(attack => Execute(attack.Message, member), x), _duration, false);
            //_buffs[member] = state;
            //member.State.AddReactiveState(state);
        }
    }

    void Execute(Attack attack, Member member)
    {
        if (member.Name.Equals(attack.Attacker.Name))
        {
            //member.State.RemoveReactiveState(_buffs[member]);
            new SimpleEffect(m => m.GainHp(attack.Damage.Calculate(attack.Attacker, attack.Target.Members[0]) * _ratio)).Apply(_performer, new Single(member));
        }
    }

}
