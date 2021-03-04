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
        
        hero.AddXp(100);
        hero.ApplyLevelUpPoint(new StatAddends().With(StatType.MaxHP, 2));
        
        Assert.AreEqual(12, hero.Stats.MaxHp());
        Assert.AreEqual(12, hero.CurrentHp);
    }
    
    [Test]
    public void Hero_XpChart_IsKnown()
    {
        var hero = new Hero(new InMemoryHeroCharacter
        {
            Class = TestClasses.Soldier,
            Stats = new StatAddends()
                .With(StatType.Damagability, 1)
                .With(StatType.MaxHP, 10)
        }, new RuntimeDeck());

        AssertLevelAfterXpGain(hero, 1, 0);
        AssertLevelAfterXpGain(hero, 2, 100);
        AssertLevelAfterXpGain(hero, 3, 200);
        AssertLevelAfterXpGain(hero, 4, 300);
        AssertLevelAfterXpGain(hero, 5, 400);
        AssertLevelAfterXpGain(hero, 6, 500);
        AssertLevelAfterXpGain(hero, 7, 600);
        AssertLevelAfterXpGain(hero, 8, 700);
        AssertLevelAfterXpGain(hero, 9, 800);
        AssertLevelAfterXpGain(hero, 10, 900);
        AssertLevelAfterXpGain(hero, 11, 1000);
        AssertLevelAfterXpGain(hero, 12, 1100);
        AssertLevelAfterXpGain(hero, 13, 1200);
        AssertLevelAfterXpGain(hero, 14, 1300);
        AssertLevelAfterXpGain(hero, 15, 1400);
    }

    private void AssertLevelAfterXpGain(Hero h, int level, int xp)
    {
        h.AddXp(xp);
        Assert.AreEqual(level, h.Level);
    }
}
