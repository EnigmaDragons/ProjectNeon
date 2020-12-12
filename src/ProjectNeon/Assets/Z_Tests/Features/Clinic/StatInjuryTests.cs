using System;
using NUnit.Framework;

public class StatInjuryTests
{
    [Test]
    public void Hero_InjuredAttackAdditive_IsCorrect()
    {
        var hero = HeroWithStats(s => s.With(StatType.Attack, 8)); 

        hero.Apply(new AdditiveStatInjury { Stat = new StringReference("Attack"), Amount = -3 });
        
        Assert.AreEqual(5, hero.Stats.Attack());
    }
    
    [Test]
    public void Hero_InjuredAttackMultiplicative_IsCorrect()
    {
        var hero = HeroWithStats(s => s.With(StatType.Attack, 8)); 

        hero.Apply(new MultiplicativeStatInjury { Stat = new StringReference("Attack"), Amount = 0.25f });
        
        Assert.AreEqual(2, hero.Stats.Attack());
    }
    
    [Test]
    public void Hero_HealInjuryByName_IsCorrect()
    {
        var hero = HeroWithStats(s => s.With(StatType.Attack, 8)); 

        hero.Apply(new MultiplicativeStatInjury { Name = new StringReference("Broken Arm"), Stat = new StringReference("Attack"), Amount = 0.25f });
        hero.HealInjuryByName("Broken Arm");
        
        Assert.AreEqual(8, hero.Stats.Attack());
    }

    private Hero HeroWithStats(Action<StatAddends> setStats)
    {
        var stats = new StatAddends().With(StatType.Damagability, 1);
        setStats(stats);
        return new Hero(new InMemoryHeroCharacter
        {
            Class = TestClasses.Soldier,
            Stats = stats
        }, new RuntimeDeck());
    }
}
