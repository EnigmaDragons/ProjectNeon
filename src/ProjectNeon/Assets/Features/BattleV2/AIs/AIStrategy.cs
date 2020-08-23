using System;
using System.Linq;

public sealed class AIStrategy
{
    private Member SingleMemberAttackTarget { get; }
    private Target GroupAttackTarget { get; }

    public AIStrategy(Member singleMemberAttackTarget, Target groupAttackTarget)
    {
        SingleMemberAttackTarget = singleMemberAttackTarget;
        GroupAttackTarget = groupAttackTarget;
    }

    public Target AttackTargetFor(CardActionSequence a) 
        => a.Scope == Scope.All ? GroupAttackTarget : new Single(SingleMemberAttackTarget);

    public Target GetRandomApplicableTarget(Member me, Member[] allConciousMembers, CardActionSequence a)
    {
        if (a.Group == Group.Self)
            return new Single(me);
        if (a.Group == Group.Ally && a.Scope == Scope.All)
            return new Multiple(allConciousMembers.Where(x => x.TeamType == me.TeamType).ToArray());
        if (a.Group == Group.Ally && a.Scope == Scope.One)
            return new Single(allConciousMembers.Where(x => x.TeamType == me.TeamType).Random());
        if (a.Group == Group.Opponent)
            return AttackTargetFor(a);
        if (a.Group == Group.All && a.Scope == Scope.All)
            return new Multiple(allConciousMembers);
        if (a.Group == Group.All && a.Scope == Scope.One)
            return new Single(allConciousMembers.Random());
        throw new Exception("Couldn't find a valid target scope, what the hell happened!");
    }
}
