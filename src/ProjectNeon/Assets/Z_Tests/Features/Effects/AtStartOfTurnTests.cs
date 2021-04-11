using NUnit.Framework;

public class AtStartOfTurnTests
{
    [Test]
    public void AtStartOfTurn_TriggersAsExpected()
    {
        var member = TestMembers.Create(m => m.With(StatType.MaxHP, 10))
            .Apply(m => m.TakeRawDamage(9));
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.AtStartOfTurn,
            NumberOfTurns = new IntReference(3),
            ReferencedSequence = TestableObjectFactory.Create<CardActionsData>().Initialized(new CardActionV2(new EffectData
            {
                EffectType = EffectType.HealFormula,
                Formula = "1"
            }))
        }, member, member);
        
        member.ExecuteStartOfTurnEffects();
        
        Assert.AreEqual(2f, member.CurrentHp());
    }
}
