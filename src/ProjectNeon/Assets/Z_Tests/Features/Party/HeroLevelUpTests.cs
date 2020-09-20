using NUnit.Framework;

public class HeroLevelUpTests
{
    [Test]
    public void Hero_LevelUpMaxHp_MaxHpAndCurrentHpCorrect()
    {
        var hero = new Hero(new InMemoryHeroCharacter
        {
            Class = TestClasses.Soldier,
            Stats = new StatAddends()
                .With(StatType.Damagability, 1)
                .With(StatType.MaxHP, 10)
        }, new RuntimeDeck());
        
        hero.ApplyLevelUpPoint(new StatAddends().With(StatType.MaxHP, 2));
        
        Assert.AreEqual(12, hero.Stats.MaxHp());
        Assert.AreEqual(12, hero.CurrentHp);
    }
}
