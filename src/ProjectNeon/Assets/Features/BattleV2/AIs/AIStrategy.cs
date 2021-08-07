using System.Collections.Generic;
using System.Linq;

public sealed class AIStrategy
{
    public Maybe<Member> SingleMemberAttackTarget { get; }
    public Target GroupAttackTarget { get; }
    public Member DesignatedAttacker { get; }
    public Dictionary<CardTag, HashSet<Target>> SelectedNonStackingTargets { get; }
    public CardTypeData DisabledCard { get; }

    public AIStrategy(Maybe<Member> singleMemberAttackTarget, Target groupAttackTarget, Member designatedAttacker, CardTypeData disabledCard)
    {
        SingleMemberAttackTarget = singleMemberAttackTarget;
        GroupAttackTarget = groupAttackTarget;
        DesignatedAttacker = designatedAttacker;
        SelectedNonStackingTargets = new Dictionary<CardTag, HashSet<Target>>();
        DisabledCard = disabledCard;
    }

    public void RecordNonStackingTarget(CardTag tag, Target target)
    {
        if (!SelectedNonStackingTargets.ContainsKey(tag))
            SelectedNonStackingTargets[tag] = new HashSet<Target>();
        SelectedNonStackingTargets[tag].Add(target);
    }

    public Target AttackTargetFor(CardActionSequence a) 
        => a.Scope == Scope.All 
            ? GroupAttackTarget 
            : SingleMemberAttackTarget.IsPresent
                ? (Target)new Single(SingleMemberAttackTarget.Value)
                : (Target)new Multiple(new Member[0]);
    
    public Target AttackTargetFor(CardActionSequence a, IEnumerable<Target> preferredTargets) 
        => a.Scope == Scope.All 
            ? GroupAttackTarget 
            : preferredTargets.Any(target => SingleMemberAttackTarget.IsPresent && target.Members[0].Id == SingleMemberAttackTarget.Value.Id) 
                ? new Single(SingleMemberAttackTarget.Value)
                : preferredTargets.Any()
                    ? preferredTargets.Random()
                    : new Multiple(new Member[0]);
    
    public AIStrategy AnticipationCopy => new AIStrategy(SingleMemberAttackTarget, GroupAttackTarget, DesignatedAttacker, DisabledCard);
}
