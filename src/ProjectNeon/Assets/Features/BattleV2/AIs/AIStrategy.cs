using System;
using System.Linq;

public sealed class AIStrategy
{
    private Member SingleMemberAttackTarget { get; }
    private Target GroupAttackTarget { get; }
    public Member DesignatedAttacker { get; }

    public AIStrategy(Member singleMemberAttackTarget, Target groupAttackTarget, Member designatedAttacker)
    {
        SingleMemberAttackTarget = singleMemberAttackTarget;
        GroupAttackTarget = groupAttackTarget;
        DesignatedAttacker = designatedAttacker;
    }

    public Target AttackTargetFor(CardActionSequence a) 
        => a.Scope == Scope.All ? GroupAttackTarget : new Single(SingleMemberAttackTarget);

    public Target GetRandomApplicableTarget(Member me, Member[] allConsciousMembers, CardActionSequence a)
    {
        if (a.Group == Group.Self)
            return new Single(me);
        if (a.Group == Group.Ally && a.Scope == Scope.All)
            return new Multiple(allConsciousMembers.Where(x => x.TeamType == me.TeamType).ToArray());
        if (a.Group == Group.Ally && a.Scope == Scope.One)
            return new Single(allConsciousMembers.Where(x => x.TeamType == me.TeamType).Random());
        if (a.Group == Group.Opponent)
            return AttackTargetFor(a);
        if (a.Group == Group.All && a.Scope == Scope.All)
            return new Multiple(allConsciousMembers);
        if (a.Group == Group.All && a.Scope == Scope.One)
            return new Single(allConsciousMembers.Random());
        if (a.Group == Group.All && a.Scope == Scope.AllExcept)
            return new Multiple(allConsciousMembers.Shuffled().Skip(1).ToArray());
        if (a.Group == Group.Ally && a.Scope == Scope.AllExcept)
            return new Multiple(allConsciousMembers.Where(x => x.TeamType == me.TeamType).ToArray().Shuffled().Skip(1).ToArray());
        if (a.Group == Group.Opponent && a.Scope == Scope.AllExcept)
            return new Multiple(allConsciousMembers.Where(x => x.TeamType != me.TeamType).ToArray().Shuffled().Skip(1).ToArray());
        throw new Exception("Couldn't find a valid target scope, what the hell happened!");
    }
}
