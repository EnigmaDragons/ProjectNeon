using System.Collections.Generic;

public sealed class EffectOnShieldBroken : Effect
{
    private readonly bool _isDebuff;
    private readonly int _maxDurationTurns;
    private readonly ReactionCardType _reaction;
    private readonly IReadOnlyDictionary<int, Member> _allMembers;

    public EffectOnShieldBroken(bool isDebuff, int maxDurationTurns, ReactionCardType reaction, IReadOnlyDictionary<int, Member> allMembers)
    {
        _isDebuff = isDebuff;
        _maxDurationTurns = maxDurationTurns;
        _reaction = reaction;
        _allMembers = allMembers;
    }

    public void Apply(Member source, Target target)
    {
        target.ApplyToAll(m =>
            m.AddReactiveState(new ReactOnShieldBroken(_isDebuff, 1, _maxDurationTurns, _allMembers, m.MemberId, source, _reaction)));
    }
}

public class ReactOnShieldBroken : ReactiveEffectV2Base
{
    public ReactOnShieldBroken(bool isDebuff, int numberOfUses, int maxDurationTurns, IReadOnlyDictionary<int, Member> allMembers, int possessingMemberId, Member originator, ReactionCardType reaction)
        : base(isDebuff, maxDurationTurns, numberOfUses, CreateMaybeEffect(allMembers, possessingMemberId, originator, reaction, effect => 
            effect.EffectData.EffectType == EffectType.Attack 
            && effect.BattleBefore.Members[possessingMemberId].State.Counters["Shield"] > 0 
            && effect.BattleAfter.Members[possessingMemberId].State.Counters["Shield"] == 0)) { }

    public override StatusTag Tag => StatusTag.None;
}