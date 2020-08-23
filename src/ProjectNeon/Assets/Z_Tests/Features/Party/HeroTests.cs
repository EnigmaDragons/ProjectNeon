using NUnit.Framework;

public class HeroTests
{
    [Test]
    public void Hero_WithGun_HasCorrectAttack()
    {
        var hero = new Hero(new InMemoryHeroCharacter
        {
            Class = TestClasses.Soldier,
            Stats = new StatAddends()
                .With(StatType.Damagability, 1)
                .With(StatType.Attack, 8)
        }, new RuntimeDeck());
        
        hero.Equip(new InMemoryEquipment
        {
            Modifiers = new EquipmentStatModifier[1]
            {
                new EquipmentStatModifier { Amount = 2, StatType = StatType.Attack.ToString(), ModifierType = StatMathOperator.Additive }
            }
        });

        var member = hero.AsMember(0);
        
        Assert.AreEqual(10, member.Attack());
    }

    [Test]
    public void Hero_EquipmentBoostsMaxHp_CurrentHpAdjustedCorrectly()
    {
        var hpBooster = new InMemoryEquipment
        {
            Modifiers = new EquipmentStatModifier[1]
            {
                new EquipmentStatModifier
                    {Amount = 2, StatType = StatType.MaxHP.ToString(), ModifierType = StatMathOperator.Additive}
            }
        };
        
        var hero = new Hero(new InMemoryHeroCharacter
        {
            Class = TestClasses.Soldier,
            Stats = new StatAddends()
                .With(StatType.MaxHP, 10)
        }, new RuntimeDeck());
        
        hero.Equip(hpBooster);
        Assert.AreEqual(12, hero.CurrentHp);
        hero.Unequip(hpBooster);
        Assert.AreEqual(10, hero.CurrentHp);
    }
}
