using NUnit.Framework;

public class EquipmentTests
{
    [Test]
    public void Equipment_WithStatModifier_IsCorrect()
    {
        var hero = new Hero(new InMemoryHeroCharacter
        {
            Class = "soldier",
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

        var member = hero.AsMemberForTests(0);
        
        Assert.AreEqual(10, member.Attack());
    }

    [Test]
    public void Equipment_BoostsMaxHp_CurrentHpAdjustedCorrectly()
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
            Class = "soldier",
            Stats = new StatAddends()
                .With(StatType.MaxHP, 10)
        }, new RuntimeDeck());
        
        hero.Equip(hpBooster);
        Assert.AreEqual(12, hero.CurrentHp);
        hero.Unequip(hpBooster);
        Assert.AreEqual(10, hero.CurrentHp);
    }

    [Test]
    public void Equipped_StartOfTurnEffect_IsApplied()
    {
        var turtleGloves = new InMemoryEquipment
        {
            TurnStartEffects = new [] 
            { 
                new EffectData
                {
                    EffectType = EffectType.AdjustStatAdditivelyFormula,
                    Formula = "1",
                    EffectScope = new StringReference(StatType.Armor.ToString()),
                    DurationFormula = "-1"
                }
            }
        };
        
        var hero = new Hero(new InMemoryHeroCharacter
        {
            Class = "soldier",
            Stats = new StatAddends()
                .With(StatType.Damagability, 1f)
                .With(StatType.MaxHP, 10)
        }, new RuntimeDeck());
        
        hero.Equip(turtleGloves);

        var member = hero.AsMemberForTests(1);
        member.ExecuteStartOfTurnEffects();
        
        Assert.AreEqual(1, member.Armor());
    }
    
    [Test]
    public void Equipment_ResourceMaxMods_IsCorrect()
    {
        var extendedClip = new InMemoryEquipment
        {
            ResourceModifiers = new IResourceType[]
            {
                new InMemoryResourceModifications 
                {
                    Name = "Ammo",
                    MaxAmount = 2,
                    StartingAmount = 2
                }
            }
        };
        var hero = new Hero(new InMemoryHeroCharacter
        {
            Class = "soldier",
            Stats = new StatAddends()
                .With(StatType.Damagability, 1f)
                .With(StatType.MaxHP, 10)
                .With(new InMemoryResourceType("Ammo") { MaxAmount = 6, StartingAmount = 6 })
        }, new RuntimeDeck());
        hero.Equip(extendedClip);
        
        var member = hero.AsMemberForTests(1);
        
        Assert.AreEqual(8, member.ResourceMax(new InMemoryResourceType("Ammo")));
        Assert.AreEqual(8, member.ResourceAmount(new InMemoryResourceType("Ammo")));
    }
}
