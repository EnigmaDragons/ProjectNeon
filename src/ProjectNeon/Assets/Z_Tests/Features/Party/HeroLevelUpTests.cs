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
        AssertLevelAfterXpGain(hero, 3, 150);
        AssertLevelAfterXpGain(hero, 4, 200);
        AssertLevelAfterXpGain(hero, 5, 250);
        AssertLevelAfterXpGain(hero, 6, 300);
        AssertLevelAfterXpGain(hero, 7, 350);
        AssertLevelAfterXpGain(hero, 8, 400);
        AssertLevelAfterXpGain(hero, 9, 450);
        AssertLevelAfterXpGain(hero, 10, 500);
        AssertLevelAfterXpGain(hero, 11, 550);
        AssertLevelAfterXpGain(hero, 12, 600);
        AssertLevelAfterXpGain(hero, 13, 650);
        AssertLevelAfterXpGain(hero, 14, 700);
        AssertLevelAfterXpGain(hero, 15, 750);
    }

    [Test]
    public void Hero_WithSmallLevel1Xp_HasLevelProgress()
    {
        var hero = new Hero(new InMemoryHeroCharacter
        {
            Class = TestClasses.Soldier,
            Stats = new StatAddends()
                .With(StatType.Damagability, 1)
                .With(StatType.MaxHP, 10)
        }, new RuntimeDeck());
        
        hero.AddXp(5);
        
        Assert.IsTrue(hero.Levels.NextLevelProgress > 0);
    }

    private void AssertLevelAfterXpGain(Hero h, int level, int xp)
    {
        h.AddXp(xp);
        Assert.AreEqual(level, h.Level);
    }
}
