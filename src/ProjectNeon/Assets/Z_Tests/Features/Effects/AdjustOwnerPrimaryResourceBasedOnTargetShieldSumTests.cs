using NUnit.Framework;

public class AdjustOwnerPrimaryResourceBasedOnTargetShieldSumTests
{
    [Test]
    public void AdjustStatAdditively_ApplyEffect_CharactersStatsAdjustedCorrectly()
    {
        var source = TestMembers.Create("Hero", TeamType.Party, x => x, new InMemoryResourceType("Energy") { MaxAmount = 99 });
        var adjustment = new EffectData
        {
            EffectType = EffectType.AdjustOwnersPrimaryResourceBasedOnTargetShieldSum, 
            Formula = "0.2"
        };
        var target = TestMembers.Create(s => s.With(StatType.MaxShield, 10).With(StatType.StartingShield, 5));

        TestEffects.Apply(adjustment, source, target);

        Assert.AreEqual(1, source.PrimaryResourceAmount());
    }
    
    [Test]
    public void AdjustStatAdditively_ApplyEffect_CharactersStatsAdjustedCorrectlyRoundsUp()
    {
        var source = TestMembers.Create("Hero", TeamType.Party, x => x, new InMemoryResourceType("Energy") { MaxAmount = 99 });
        var adjustment = new EffectData
        {
            EffectType = EffectType.AdjustOwnersPrimaryResourceBasedOnTargetShieldSum, 
            Formula = "0.22"
        };
        var target = TestMembers.Create(s => s.With(StatType.MaxShield, 10).With(StatType.StartingShield, 5));

        TestEffects.Apply(adjustment, source, target);

        Assert.AreEqual(2, source.PrimaryResourceAmount());
    }
}
