using System.Collections.Generic;

public sealed class EffectOnAvoided : Effect
{
    private readonly bool _isDebuff;
    private readonly int _numberOfUses;
    private readonly int _maxDurationTurns;
    private readonly StatusDetail _status;
    private readonly ReactiveTriggerScope _triggerScope;
    private readonly AvoidanceType _avoidanceType;
    private readonly ReactionCardType _reaction;

    public EffectOnAvoided(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusDetail status, ReactiveTriggerScope triggerScope, AvoidanceType avoidanceType, ReactionCardType reaction)
    {
        _isDebuff = isDebuff;
        _numberOfUses = numberOfUses;
        _maxDurationTurns = maxDurationTurns;
        _status = status;
        _triggerScope = triggerScope;
        _avoidanceType = avoidanceType;
        _reaction = reaction;
    }

    public void Apply(EffectContext ctx)
    {
        ctx.Target.ApplyToAllConscious(m =>
            m.AddReactiveState(new ReactOnAvoided(_isDebuff, _numberOfUses, _maxDurationTurns, _status, _triggerScope, _avoidanceType, ctx.BattleMembers, m.MemberId, ctx.Source, _reaction)));
    }
}

public sealed class ReactOnAvoided : ReactiveEffectV2Base
{
    public ReactOnAvoided(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusDetail status, ReactiveTriggerScope triggerScope, AvoidanceType avoidanceType, 
        IDictionary<int, Member> allMembers, int possessingMemberId, Member originator, ReactionCardType reaction)
        : base(isDebuff, maxDurationTurns, numberOfUses, status, CreateMaybeAvoidedEffect(allMembers, possessingMemberId, originator, reaction, 
            avoidedAction => avoidedAction.Avoid.Type == avoidanceType && triggerScope.IsInTriggerScope(originator, allMembers[possessingMemberId], avoidedAction.Source))) {}
}
