using System.Collections.Generic;

public sealed class EffectOnDamaged : Effect
{
    private readonly bool _isDebuff;
    private readonly int _numberOfUses;
    private readonly int _maxDurationTurns;
    private readonly ReactionCardType _reaction;
    private readonly IReadOnlyDictionary<int, Member> _allMembers;

    public EffectOnDamaged(bool isDebuff, int maxDurationTurns, int numberOfUses, ReactionCardType reaction, IReadOnlyDictionary<int, Member> allMembers)
    {
        _isDebuff = isDebuff;
        _numberOfUses = numberOfUses;
        _maxDurationTurns = maxDurationTurns;
        _reaction = reaction;
        _allMembers = allMembers;
    }

    public void Apply(Member source, Target target)
    {
        target.ApplyToAllConscious(m =>
            m.AddReactiveState(new ReactOnDamaged(_isDebuff, _numberOfUses, _maxDurationTurns, _allMembers, m.MemberId, source, _reaction)));
    }
}

public class ReactOnDamaged : ReactiveEffectV2Base
{
    public ReactOnDamaged(bool isDebuff, int numberOfUses, int maxDurationTurns, IReadOnlyDictionary<int, Member> allMembers, int possessingMemberId, Member originator, ReactionCardType reaction)
        : base(isDebuff, maxDurationTurns, numberOfUses, CreateMaybeEffect(allMembers, possessingMemberId, originator, reaction, effect => 
            effect.BattleBefore.Members[possessingMemberId].State.Counters["HP"] > effect.BattleAfter.Members[possessingMemberId].State.Counters["HP"])) { }

    public override StatusTag Tag => StatusTag.None;
}