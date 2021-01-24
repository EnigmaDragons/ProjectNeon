using System.Collections.Generic;

public sealed class EffectOnDamaged : Effect
{
    private readonly bool _isDebuff;
    private readonly int _numberOfUses;
    private readonly int _maxDurationTurns;
    private readonly ReactionCardType _reaction;

    public EffectOnDamaged(bool isDebuff, int maxDurationTurns, int numberOfUses, ReactionCardType reaction)
    {
        _isDebuff = isDebuff;
        _numberOfUses = numberOfUses;
        _maxDurationTurns = maxDurationTurns;
        _reaction = reaction;
    }

    public void Apply(EffectContext ctx)
    {
        ctx.Target.ApplyToAllConscious(m =>
            m.AddReactiveState(new ReactOnDamaged(_isDebuff, _numberOfUses, _maxDurationTurns, ctx.BattleMembers, m.MemberId, ctx.Source, _reaction)));
    }
}

public class ReactOnDamaged : ReactiveEffectV2Base
{
    public ReactOnDamaged(bool isDebuff, int numberOfUses, int maxDurationTurns, IDictionary<int, Member> allMembers, int possessingMemberId, Member originator, ReactionCardType reaction)
        : base(isDebuff, maxDurationTurns, numberOfUses, new StatusDetail(StatusTag.OnDamaged), CreateMaybeEffect(allMembers, possessingMemberId, originator, false, reaction, effect => 
            effect.BattleBefore.Members[possessingMemberId].State.Counters["HP"] > effect.BattleAfter.Members[possessingMemberId].State.Counters["HP"])) { }
}