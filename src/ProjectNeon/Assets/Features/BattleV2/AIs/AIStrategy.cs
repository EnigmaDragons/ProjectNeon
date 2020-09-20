using System;
using System.Collections.Generic;
using System.Linq;

public sealed class AIStrategy
{
    private Member SingleMemberAttackTarget { get; }
    private Target GroupAttackTarget { get; }
    public Member DesignatedAttacker { get; }
    public Dictionary<CardTag, HashSet<Target>> SelectedNonStackingTargets { get; } 

    public AIStrategy(Member singleMemberAttackTarget, Target groupAttackTarget, Member designatedAttacker)
    {
        SingleMemberAttackTarget = singleMemberAttackTarget;
        GroupAttackTarget = groupAttackTarget;
        DesignatedAttacker = designatedAttacker;
        SelectedNonStackingTargets = new Dictionary<CardTag, HashSet<Target>>();
    }

    public void RecordNonStackingTarget(CardTag tag, Target target)
    {
        if (!SelectedNonStackingTargets.ContainsKey(tag))
            SelectedNonStackingTargets[tag] = new HashSet<Target>();
        SelectedNonStackingTargets[tag].Add(target);
    }

    public Target AttackTargetFor(CardActionSequence a) 
        => a.Scope == Scope.All ? GroupAttackTarget : new Single(SingleMemberAttackTarget);
}
