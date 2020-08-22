using System.Linq;

public sealed class EffectOnEvaded : Effect
{
    private readonly bool _isDebuff;
    private readonly int _numberOfUses;
    private readonly int _maxDurationTurns;
    private readonly ReactionCardType _reaction;

    public EffectOnEvaded(bool isDebuff, int numberOfUses, int maxDurationTurns, ReactionCardType reaction)
    {
        _isDebuff = isDebuff;
        _numberOfUses = numberOfUses;
        _maxDurationTurns = maxDurationTurns;
        _reaction = reaction;
    }

    public void Apply(Member source, Target target)
    {
        target.ApplyToAll(m =>
            m.AddReactiveState(new ReactOnEvaded(_isDebuff, _numberOfUses, _maxDurationTurns, m.MemberId, source, _reaction)));
    }
}

public sealed class ReactOnEvaded : ReactiveEffectV2Base
{
    public ReactOnEvaded(bool isDebuff, int numberOfUses, int maxDurationTurns, int possessingMemberId, Member originator, ReactionCardType reaction)
        : base(isDebuff, maxDurationTurns, numberOfUses, effect =>
        {
            var reactingMaybeMember = effect.Target.Members.Where(m => m.Id == possessingMemberId);
            if (effect.EffectData.EffectType != EffectType.Attack || reactingMaybeMember.None() || effect.BattleBefore.Members[reactingMaybeMember.Single().Id].State.Counters["Evade"] < effect.BattleAfter.Members[reactingMaybeMember.Single().Id].State.Counters["Evade"])
                return Maybe<ProposedReaction>.Missing();

            var possessor = reactingMaybeMember.First();
            var action = reaction.ActionSequence;
            var reactor = action.Reactor == ReactiveMember.Originator ? originator : possessor;

            var target = possessor;
            if (action.Scope == ReactiveTargetScope.Attacker)
                target = effect.Source;
            // TODO: Implement other scopes

            return new ProposedReaction(reaction, reactor, target);
        })
    {
    }

    public override StatusTag Tag => StatusTag.CounterAttack;
}