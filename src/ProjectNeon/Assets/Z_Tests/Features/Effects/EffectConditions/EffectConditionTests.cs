using NUnit.Framework;

[TestFixture]
public class EffectConditionTests
{
    [TestCase]
    public void TargetIsEnemy_IsEnemy_NoReasons()
    {
        AssertConditionSucceeded(GetShouldNotApplyReason<TargetIsEnemy>(TestMembers.AnyEnemy()));
    }

    [TestCase]
    public void TargetIsEnemy_IsNotEnemy_HasReasons()
    {
        AssertConditionFailed(GetShouldNotApplyReason<TargetIsEnemy>(TestMembers.AnyAlly()));
    }

    [TestCase]
    public void TargetIsAlly_IsAlly_NoReasons()
    {
        AssertConditionSucceeded(GetShouldNotApplyReason<TargetIsAlly>(TestMembers.AnyAlly()));
    }

    [TestCase]
    public void TargetIsAlly_IsNotAlly_HasReasons()
    {
        AssertConditionFailed(GetShouldNotApplyReason<TargetIsAlly>(TestMembers.AnyEnemy()));
    }

    [TestCase]
    public void TargetIsAfflicted_IsAfflicted_NoReasons()
    {
        var member = TestMembers.Any();
        member.Apply(s => s.ApplyTemporaryAdditive(new DamageOverTimeState(member.Id, 5, member, 5)));

        var reason = GetShouldNotApplyReason<TargetIsAfflicted>(member);

        AssertConditionSucceeded(reason);
    }
    
    [TestCase]
    public void TargetIsAfflicted_NotAfflicted_HasReasons()
    {
        AssertConditionFailed(GetShouldNotApplyReason<TargetIsAfflicted>(TestMembers.Any()));
    }

    private void AssertConditionSucceeded(Maybe<string> reason) => Assert.IsTrue(reason.IsMissing);
    private void AssertConditionFailed(Maybe<string> reason) => Assert.IsTrue(reason.IsPresent);
        
    private Maybe<string> GetShouldNotApplyReason<T>(Member m) 
        where T : EffectCondition 
            => TestableObjectFactory.Create<T>().GetShouldNotApplyReason(ForTarget(new Single(m)));

    private EffectContext ForTarget(Target t) => EffectContext.ForTests(TestMembers.Any(), t, Maybe<Card>.Missing(),
        ResourceQuantity.None, new UnpreventableContext());
}