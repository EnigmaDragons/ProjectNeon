using System.Collections.Generic;

public sealed class EffectOnAttacked : Effect
{
    private readonly bool _isDebuff;
    private readonly int _numberOfUses;
    private readonly int _maxDurationTurns;
    private readonly StatusTag _tag;
    private readonly ReactiveTriggerScope _triggerScope;
    private readonly ReactionCardType _reaction;

    public EffectOnAttacked(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusTag tag, ReactiveTriggerScope triggerScope, ReactionCardType reaction)
    {
        _isDebuff = isDebuff;
        _numberOfUses = numberOfUses;
        _maxDurationTurns = maxDurationTurns;
        _tag = tag;
        _triggerScope = triggerScope;
        _reaction = reaction;
    }
    
    public void Apply(EffectContext ctx)
    {
        ctx.Target.ApplyToAllConscious(m => 
            m.AddReactiveState(new ReactOnAttacked(_isDebuff, _numberOfUses, _maxDurationTurns, _tag, _triggerScope, ctx.BattleMembers, m.MemberId, ctx.Source, _reaction)));
    }
}

public sealed class ReactOnAttacked : ReactiveEffectV2Base
{
    public ReactOnAttacked(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusTag tag, ReactiveTriggerScope triggerScope, 
        IDictionary<int, Member> allMembers, int possessingMemberId, Member originator, ReactionCardType reaction)
        : base(isDebuff, maxDurationTurns, numberOfUses, CreateMaybeEffect(allMembers, possessingMemberId, originator, false, reaction, 
            effect => effect.EffectData.EffectType == EffectType.Attack && triggerScope.IsInTriggerScope(originator, allMembers[possessingMemberId], effect.Source)))
    {
        Tag = tag;
    }

    public override StatusTag Tag { get; }
}
