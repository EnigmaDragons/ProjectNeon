using System.Linq;

public sealed class EffectOnAttacked : Effect
{
    private readonly bool _isDebuff;
    private readonly int _numberOfUses;
    private readonly int _maxDurationTurns;
    private readonly ReactionCardType _reaction;

    public EffectOnAttacked(bool isDebuff, int numberOfUses, int maxDurationTurns, ReactionCardType reaction)
    {
        _isDebuff = isDebuff;
        _numberOfUses = numberOfUses;
        _maxDurationTurns = maxDurationTurns;
        _reaction = reaction;
    }
    
    public void Apply(Member source, Target target)
    {
        target.ApplyToAll(m => 
            m.AddReactiveState(new ReactOnAttacked(_isDebuff, _numberOfUses, _maxDurationTurns, m.MemberId, source, _reaction)));
    }
}

public sealed class ReactOnAttacked : ReactiveEffectV2Base
{
    public ReactOnAttacked(bool isDebuff, int numberOfUses, int maxDurationTurns, int possessingMemberId, Member originator, ReactionCardType reaction)
        : base(isDebuff, maxDurationTurns, numberOfUses, effect =>
        {
            // TODO: Need to be able to target the Reaction Effect. Should it be attached member? Attacker? Originator?
            var reactingMaybeMember = effect.Target.Members.Where(m => m.Id == possessingMemberId);
            if (effect.EffectData.EffectType != EffectType.Attack || reactingMaybeMember.None())
                return Maybe<ProposedReaction>.Missing();

            var action = reaction.ActionSequence;
            var reactor = action.Reactor == ReactiveMember.Originator ? originator : reactingMaybeMember.First();
            return effect.EffectData.EffectType == EffectType.Attack && reactingMaybeMember.Any()
                ? new ProposedReaction(reaction, reactor, reactingMaybeMember.First())
                : Maybe<ProposedReaction>.Missing();
        })
    {
    }
}
