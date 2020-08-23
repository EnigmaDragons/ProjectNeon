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
}
