using System.Linq;

public sealed class EffectOnAttacked : Effect
{
    private readonly bool _isDebuff;
    private readonly int _numberOfUses;
    private readonly int _maxDurationTurns;
    private readonly CardActionsData _reaction;

    public EffectOnAttacked(bool isDebuff, int numberOfUses, int maxDurationTurns, CardActionsData reaction)
    {
        _isDebuff = isDebuff;
        _numberOfUses = numberOfUses;
        _maxDurationTurns = maxDurationTurns;
        _reaction = reaction;
    }
    
    public void Apply(Member source, Target target)
    {
        target.ApplyToAll(m => 
            m.AddReactiveState(new ReactOnAttacked(_isDebuff, _numberOfUses, _maxDurationTurns, m.MemberId, _reaction)));
    }
}

public sealed class ReactOnAttacked : ReactiveEffectV2Base
{
    public ReactOnAttacked(bool isDebuff, int numberOfUses, int maxDurationTurns, int triggeringMemberId, CardActionsData reaction)
        : base(isDebuff, maxDurationTurns, numberOfUses, effect =>
        {
            // TODO: Need to be able to target the Reaction Effect. Should it be attached member? Attacker? Originator?
            var reactingMaybeMember = effect.Target.Members.Where(m => m.Id == triggeringMemberId);
            return effect.EffectData.EffectType == EffectType.Attack && reactingMaybeMember.Any()
                ? new ProposedEffect(reaction, reactingMaybeMember.First(), reactingMaybeMember.First())
                : Maybe<ProposedEffect>.Missing();
        })
    {
    }
}
