﻿using System.Collections.Generic;
using System.Linq;

public sealed class EffectOnEvaded : Effect
{
    private readonly bool _isDebuff;
    private readonly int _numberOfUses;
    private readonly int _maxDurationTurns;
    private readonly ReactiveTriggerScope _triggerScope;
    private readonly ReactionCardType _reaction;

    public EffectOnEvaded(bool isDebuff, int numberOfUses, int maxDurationTurns, ReactiveTriggerScope triggerScope, ReactionCardType reaction)
    {
        _isDebuff = isDebuff;
        _numberOfUses = numberOfUses;
        _maxDurationTurns = maxDurationTurns;
        _triggerScope = triggerScope;
        _reaction = reaction;
    }

    public void Apply(EffectContext ctx)
    {
        ctx.Target.ApplyToAllConscious(m =>
            m.AddReactiveState(new ReactOnEvaded(_isDebuff, _numberOfUses, _maxDurationTurns, _triggerScope, ctx.BattleMembers, m.MemberId, ctx.Source, _reaction)));
    }
}

public sealed class ReactOnEvaded : ReactiveEffectV2Base
{
    public ReactOnEvaded(bool isDebuff, int numberOfUses, int maxDurationTurns, ReactiveTriggerScope triggerScope, IDictionary<int, Member> allMembers, int possessingMemberId, Member originator, ReactionCardType reaction)
        : base(isDebuff, maxDurationTurns, numberOfUses, CreateMaybeEffect(allMembers, possessingMemberId, originator, reaction, effect => 
            effect.EffectData.EffectType == EffectType.Attack 
            && effect.BattleBefore.Members[possessingMemberId].State.Counters["Evade"] > effect.BattleAfter.Members[possessingMemberId].State.Counters["Evade"]
            && triggerScope.IsInTriggerScope(originator, allMembers[possessingMemberId], effect.Source))) {}

    public override StatusTag Tag => StatusTag.CounterAttack;
}