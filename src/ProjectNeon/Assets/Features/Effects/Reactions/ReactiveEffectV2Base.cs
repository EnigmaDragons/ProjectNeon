using System;
using System.Collections.Generic;
using System.Linq;

public abstract class ReactiveEffectV2Base : ReactiveStateV2
{
    private int _remainingDurationTurns;
    private int _remainingUses;
    private readonly Func<EffectResolved, Maybe<ProposedReaction>> _createMaybeEffect;
    private bool HasMoreUses => _remainingUses != 0;
    private bool HasMoreTurns => _remainingDurationTurns != 0;

    public IStats Stats => new StatAddends();
    public bool IsDebuff { get; }
    public bool IsActive => HasMoreUses && HasMoreTurns;

    public ReactiveEffectV2Base(bool isDebuff, int maxDurationTurns, int maxUses, Func<EffectResolved, Maybe<ProposedReaction>> createMaybeEffect)
    {
        _remainingDurationTurns = maxDurationTurns;
        _remainingUses = maxUses;
        _createMaybeEffect = createMaybeEffect;
        IsDebuff = isDebuff;
    }
    
    public void OnTurnStart() {}

    public void OnTurnEnd()
    {
        if (_remainingDurationTurns > -1)
            _remainingDurationTurns--;
    }

    public abstract StatusTag Tag { get; }

    public Maybe<ProposedReaction> OnEffectResolved(EffectResolved e)
    {
        var maybeEffect = _createMaybeEffect(e);
        if (maybeEffect.IsPresent)
            _remainingUses--;
        return maybeEffect;
    }

    protected static Func<EffectResolved, Maybe<ProposedReaction>> CreateMaybeEffect(IDictionary<int, Member> members, int possessingMemberId, Member originator, ReactionCardType reaction, Func<EffectResolved, bool> condition) => 
        effect =>
        {
            var reactingMaybeMember = effect.Target.Members.Where(m => m.Id == possessingMemberId);
            if (reactingMaybeMember.None() || !condition(effect))
                return Maybe<ProposedReaction>.Missing();

            var possessor = reactingMaybeMember.First();
            var action = reaction.ActionSequence;
            var reactor = action.Reactor == ReactiveMember.Originator ? originator : possessor;

            Target target = new Single(possessor);
            if (action.Scope == ReactiveTargetScope.Attacker)
                target = new Single(effect.Source);
            if (action.Scope == ReactiveTargetScope.AllEnemies)
                target = new Multiple(members.Values.Where(x => x.IsConscious() && x.TeamType == TeamType.Enemies).ToArray());
            // TODO: Implement other scopes

            return new ProposedReaction(reaction, reactor, target);
        };
}
