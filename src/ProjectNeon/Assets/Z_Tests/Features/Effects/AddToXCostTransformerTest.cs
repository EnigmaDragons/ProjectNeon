using NUnit.Framework;

public class AddToXCostTransformerTest
{
    [Test]
    public void AddToXCostTransformer_AppliesToXCostFormulas()
    {
        var member = TestMembers.Create("Bob", x => x.With(StatType.MaxHP, 10));
        member.State.AddEffectTransformer(new AddToXCostTransformer(new EffectData { FloatAmount = new FloatReference(-1) }, -1));
        var effect1 = new EffectData
        {
            EffectType = EffectType.AttackFormula,
            Formula = "1"
        };
        var effect2 = new EffectData
        {
            EffectType = EffectType.AttackFormula,
            Formula = "2 * X"
        };

        TestEffects.Apply(effect1, member, member, new Card(1, member, TestableObjectFactory.Create<CardType>()), new ResourceQuantity { Amount = 2 });
        TestEffects.Apply(effect2, member, member, new Card(1, member, TestableObjectFactory.Create<CardType>()), new ResourceQuantity { Amount = 2 });

        Assert.AreEqual(3, member.CurrentHp());
    }
}