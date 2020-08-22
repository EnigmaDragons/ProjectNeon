using NUnit.Framework;

public class AdjustStatsAdditivelyTests
{
    [Test]
    public void AdjustStatAdditively_ApplyEffect_CharactersStatsAdjusted()
    {
        var adjustment = new EffectData { EffectType = EffectType.AdjustStatAdditively, EffectScope = new StringReference("Attack"), FloatAmount = new FloatReference(1) };
        var target = TestMembers.Create(s => s.With(StatType.Attack, 10));

        AllEffects.Apply(adjustment, TestMembers.Any(), target);

        Assert.AreEqual(11, target.State[StatType.Attack]);
    }
}