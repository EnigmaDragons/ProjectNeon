using System.Collections.Generic;
using System.Linq;

public sealed class AIStrategy
{
    public Maybe<Member> SingleMemberAttackTarget { get; }
    public Target GroupAttackTarget { get; }
    public Member DesignatedAttacker { get; }
    public EnemySpecialCircumstanceCards SpecialCards { get; }
    public Dictionary<CardTag, HashSet<Target>> SelectedNonStackingTargets { get; }
    public DeterministicRng Rng { get; }
    public bool ShouldRegenerate { get; private set; }

    public AIStrategy(Maybe<Member> singleMemberAttackTarget, Target groupAttackTarget, Member designatedAttacker, EnemySpecialCircumstanceCards specialCards, DeterministicRng rng)
    {
        SingleMemberAttackTarget = singleMemberAttackTarget;
        GroupAttackTarget = groupAttackTarget;
        DesignatedAttacker = designatedAttacker;
        SpecialCards = specialCards;
        SelectedNonStackingTargets = new Dictionary<CardTag, HashSet<Target>>();
        Rng = rng;
    }

    public void RecordNonStackingTarget(CardTag tag, Target target)
    {
        if (!SelectedNonStackingTargets.ContainsKey(tag))
            SelectedNonStackingTargets[tag] = new HashSet<Target>();
        SelectedNonStackingTargets[tag].Add(target);
    }

    public void TriggerStrategyRegeneration() => ShouldRegenerate = true;

    public Target AttackTargetFor(Target[] possibleTargets, CardActionSequence a) 
        => a.Scope == Scope.All
            ? GroupAttackTarget
            : SingleMemberAttackTarget.IsPresent
                ? new Single(SingleMemberAttackTarget.Value)
                : possibleTargets.Any() 
                    ? possibleTargets.Random(Rng)
                    : new NoTarget();
    
    public Target AttackTargetFor(Target[] possibleTargets, CardActionSequence a, IEnumerable<Target> preferredTargets) 
        => a.Scope == Scope.All 
            ? GroupAttackTarget 
            : preferredTargets.Any(target => SingleMemberAttackTarget.IsPresent && target.Members[0].Id == SingleMemberAttackTarget.Value.Id) 
                ? new Single(SingleMemberAttackTarget.Value)
                : preferredTargets.Any()
                    ? preferredTargets.Random(Rng)
                    : possibleTargets.Any() 
                        ? possibleTargets.Random(Rng)
                        : new NoTarget();
    
    public AIStrategy AnticipationCopy => new AIStrategy(SingleMemberAttackTarget, GroupAttackTarget, DesignatedAttacker, SpecialCards, new DeterministicRng(Rng.Seed));
}
