using System.Collections.Generic;
using UnityEngine;

public sealed class EffectOnDeath : Effect
{
    private readonly bool _isDebuff;
    private readonly int _numberOfUses;
    private readonly string _maxDurationFormula;
    private readonly ReactionCardType _reaction;
    private readonly ReactionTimingWindow _timing;
    private readonly EffectData _e;

    public EffectOnDeath(bool isDebuff, int numberOfUses, string maxDurationFormula, ReactionCardType reaction, ReactionTimingWindow timing, EffectData e)
    {
        _isDebuff = isDebuff;
        _numberOfUses = numberOfUses;
        _maxDurationFormula = maxDurationFormula;
        _reaction = reaction;
        _timing = timing;
        _e = e;
    }
    
    public void Apply(EffectContext ctx)
    {
        ctx.Target.ApplyToAllConscious(m => 
            m.AddReactiveState(new ReactOnDeath(_isDebuff, _numberOfUses, Formula.EvaluateToInt(ctx.SourceSnapshot.State, m, 
                _maxDurationFormula, ctx.XPaidAmount, ctx.ScopedData), ctx.BattleMembers, m.MemberId, ctx.Source, _reaction, _timing, 
                    string.IsNullOrWhiteSpace(_e.StatusDetailTerm.ToLocalized()) ? Maybe<string>.Missing() : _e.StatusDetailTerm.ToLocalized())));
    }
}

public sealed class ReactOnDeath : ReactiveEffectV2Base
{
    public ReactOnDeath(bool isDebuff, int numberOfUses, int maxDurationTurns, IDictionary<int, Member> allMembers, int possessingMemberId, Member originator, ReactionCardType reaction, 
        ReactionTimingWindow timing, Maybe<string> statusDetailTerm)
        : base(originator.Id, isDebuff, maxDurationTurns, numberOfUses, new StatusDetail(StatusTag.WhenKilled, statusDetailTerm), timing, false, CreateMaybeEffects(allMembers, possessingMemberId, originator, true, reaction, timing,
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
                        TemporalStateMetadata.BuffForDuration(originator.Id, 1, new StatusDetail(StatusTag.Invulnerable, Maybe<string>.Missing()))));
                }
                return result ? 1 : 0;
            })) {}
}