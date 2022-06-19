using NUnit.Framework;

public class HeroHealthTests
{
    [Test]
    public void HeroHealth_WithInjuryAndFinalBattleStats_HealthNotAboveMaxAdjustedHp()
    {
        var hero = new Hero(new InMemoryHeroCharacter { Stats = new StatAddends()
            .With(StatType.MaxHP, 20)
            .With(TemporalStatType.HP, 20)}, new RuntimeDeck());
        
        hero.Apply(new AdditiveStatInjury { Stat = new StringReference("MaxHP"), Amount = -5 });
        hero.SetHp(20);
        
        Assert.AreEqual(15, hero.CurrentHp);
    }
}
