using NUnit.Framework;

public class CheckNamedConditionTests
{
    [Test]
    public void CheckNamedConditionWithoutEverRecordingCondition()
    {
        
        var condition = TestableObjectFactory.Create<CheckNameCondition>();
        condition.conditionName = "ourUnmetCondition";
        var reason = condition.GetShouldNotApplyReason(EffectContext.ForTests(TestMembers.Any(), new Single(TestMembers.Any())));
        Assert.IsTrue(reason.IsPresent);
    }
    [Test]
    public void CheckNamedCondition_EffectDoesNotApply()
    {
        var condition = TestableObjectFactory.Create<CheckNameCondition>();
        condition.conditionName = "ourUnmetCondition";
        var attacker = TestMembers.Create(s => s.With(StatType.Attack, 1f));
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10));
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.AttackFormula,
            Formula = "1 * Attack",
            Conditions = condition.AsArray()
        }, attacker, target);

        Assert.AreEqual(10, target.State[TemporalStatType.HP]);
    }
}