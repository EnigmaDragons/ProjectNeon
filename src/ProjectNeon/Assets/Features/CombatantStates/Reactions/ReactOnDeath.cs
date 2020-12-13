using System.Collections.Generic;

public sealed class EffectOnDeath : Effect
{
    private readonly bool _isDebuff;
    private readonly int _numberOfUses;
    private readonly int _maxDurationTurns;
    private readonly ReactionCardType _reaction;

    public EffectOnDeath(bool isDebuff, int numberOfUses, int maxDurationTurns, ReactionCardType reaction)
    {
        _isDebuff = isDebuff;
        _numberOfUses = numberOfUses;
        _maxDurationTurns = maxDurationTurns;
        _reaction = reaction;
    }
    
    public void Apply(EffectContext ctx)
    {
        ctx.Target.ApplyToAllConscious(m => 
            m.AddReactiveState(new ReactOnDeath(_isDebuff, _numberOfUses, _maxDurationTurns, ctx.BattleMembers, m.MemberId, ctx.Source, _reaction)));
    }
}

public sealed class ReactOnDeath : ReactiveEffectV2Base
{
    public ReactOnDeath(bool isDebuff, int numberOfUses, int maxDurationTurns, IDictionary<int, Member> allMembers, int possessingMemberId, Member originator, ReactionCardType reaction)
        : base(isDebuff, maxDurationTurns, numberOfUses, CreateMaybeEffect(allMembers, possessingMemberId, originator, reaction, false,
            effect => !allMembers[possessingMemberId].IsConscious())) {}

    public override StatusTag Tag => StatusTag.OnDeath;
}