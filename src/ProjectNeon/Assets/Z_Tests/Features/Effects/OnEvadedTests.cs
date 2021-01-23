using NUnit.Framework;

public class OnEvadedTests
{
    [Test, Ignore("Need to be updated to use new Avoidance System.")]
    public void OnEvaded_ApplyTwice_OnlyGeneratesOneReaction()
    {
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10));
        var attacker = TestMembers.Create(s => s.With(StatType.Attack, 1));
        target.State.AdjustEvade(1);

        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.OnEvaded,
            NumberOfTurns = new IntReference(3),
            FloatAmount = new FloatReference(-1),
            ReactionCard = TestCards.ReactionCard(
                ReactiveMember.Possessor,
                ReactiveTargetScope.Self,
                new EffectData
                {
                    EffectType = EffectType.AdjustStatAdditivelyFormula, 
                    Formula = "1", 
                    EffectScope = new StringReference("Armor"), 
                    NumberOfTurns = new IntReference(-1)
                })
        }, target, target);

        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.Attack,
            FloatAmount = new FloatReference(1)
        }, attacker, target);

        Assert.AreEqual(10, target.State[TemporalStatType.HP]);
        Assert.AreEqual(1, target.State.Armor());
    }
}
