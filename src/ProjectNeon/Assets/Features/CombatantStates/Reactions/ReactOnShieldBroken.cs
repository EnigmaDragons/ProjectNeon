using System.Collections.Generic;

public sealed class EffectOnShieldBroken : Effect
{
    private readonly bool _isDebuff;
    private readonly int _maxDurationTurns;
    private readonly ReactiveTriggerScope _triggerScope;
    private readonly ReactionCardType _reaction;

    public EffectOnShieldBroken(bool isDebuff, int maxDurationTurns, ReactiveTriggerScope triggerScope, ReactionCardType reaction)
    {
        _isDebuff = isDebuff;
        _maxDurationTurns = maxDurationTurns;
        _reaction = reaction;
    }
    
    public void Apply(EffectContext ctx)
    {
        ctx.Target.ApplyToAllConscious(m =>
            m.AddReactiveState(new ReactOnShieldBroken(_isDebuff, 1, _maxDurationTurns, ctx.BattleMembers, m.MemberId, ctx.Source, _reaction)));
    }
}

public class ReactOnShieldBroken : ReactiveEffectV2Base
{
    public ReactOnShieldBroken(bool isDebuff, int numberOfUses, int maxDurationTurns, IDictionary<int, Member> allMembers, int possessingMemberId, Member originator, ReactionCardType reaction)
        : base(isDebuff, maxDurationTurns, numberOfUses, CreateMaybeEffect(allMembers, possessingMemberId, originator, false, reaction, effect => 
            effect.EffectData.EffectType == EffectType.Attack 
            && effect.BattleBefore.Members[possessingMemberId].State.Counters["Shield"] > 0 
            && effect.BattleAfter.Members[possessingMemberId].State.Counters["Shield"] == 0)) { }

    public override StatusTag Tag => StatusTag.None;
}