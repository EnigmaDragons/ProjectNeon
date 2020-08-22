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
}
