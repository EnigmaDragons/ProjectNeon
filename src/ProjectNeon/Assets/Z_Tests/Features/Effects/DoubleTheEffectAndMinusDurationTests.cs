using NUnit.Framework;

public class DoubleTheEffectAndMinusDurationTests
{
    [Test]
    public void OnlyAppliesToEffectsWithDuration() 
    {
        var member = TestMembers.Create(s => s.With(StatType.Attack, 0f).With(StatType.MaxHP, 10).With(StatType.Magic, 1));
        member.State.AddEffectTransformer(new DoubleTheEffectAndMinus1DurationTransformer(new EffectData { NumberOfTurns = new IntReference(-1), FloatAmount = new FloatReference(-1) }));
        var effect1 = new EffectData
        {
            EffectType = EffectType.AdjustStatAdditivelyFormula,
            EffectScope = new StringReference("Attack"),
            NumberOfTurns = new IntReference(2),
            Formula = "1"
        };
        var effect2 = new EffectData
        {
            EffectType = EffectType.MagicDamageOverTime,
            FloatAmount = new FloatReference(1),
            NumberOfTurns = new IntReference(2),
        };
        var effect3 = new EffectData
        {
            EffectType = EffectType.AdjustStatAdditivelyFormula,
            EffectScope = new StringReference("Attack"),
            NumberOfTurns = new IntReference(-1),
            Formula = "1"
        };
        
        TestEffects.Apply(effect1, member, member, new Card(1, member, new CardType()));
        TestEffects.Apply(effect2, member, member, new Card(1, member, new CardType()));
        TestEffects.Apply(effect3, member, member, new Card(1, member, new CardType()));

        Assert.AreEqual(3, member.Attack());
        member.State.GetTurnStartEffects();
        member.State.GetTurnEndEffects();
        Assert.AreEqual(1, member.Attack());
        Assert.AreEqual(8, member.CurrentHp());
        member.State.GetTurnStartEffects();
        member.State.GetTurnEndEffects();
        Assert.AreEqual(8, member.CurrentHp());
    }
}