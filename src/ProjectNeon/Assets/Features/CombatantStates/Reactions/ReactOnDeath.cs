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
        : base(isDebuff, maxDurationTurns, numberOfUses, CreateMaybeEffect(allMembers, possessingMemberId, originator, reaction, 
            effect =>
            {
                //this is super hacky but the amount of changes required to bypass the consciousness system turns out to be completely insane
                //so when using death cards that you want to have still die, make sure to use the suicide effect which doesn't care about targets
                var member = allMembers[possessingMemberId];
                var result = !member.IsConscious();
                if (result)
                {
                    member.State.SetHp(1);
                    member.State.ApplyTemporaryMultiplier(new AdjustedStats(
                        new StatMultipliers().With(StatType.Damagability, 0f),
                        TemporalStateMetadata.BuffForDuration(1, StatusTag.Invulnerable)));
                }
                return result;
            })) {}

    public override StatusTag Tag => StatusTag.OnDeath;
}