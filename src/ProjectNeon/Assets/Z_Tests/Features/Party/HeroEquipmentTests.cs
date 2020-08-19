using NUnit.Framework;

public class HeroEquipmentTests
{
    [Test]
    public void HeroEquipment_New_NoEquipment()
    {
        var heroEquipment = new HeroEquipment(TestClasses.Soldier);
        
        Assert.AreEqual(0, heroEquipment.All.Length);
    }
    
    [Test]
    public void HeroEquipment_CanEquipForNonMatchingClass_IsFalse()
    {
        var heroEquipment = new HeroEquipment(TestClasses.Soldier);

        var sword = TestableObjectFactory.Create<Equipment>().Initialized(TestClasses.Paladin);
        
        Assert.IsFalse(heroEquipment.CanEquip(sword));
    }
    
    [Test]
    public void HeroEquipment_CanEquipForMatchingClass_IsTrue()
    {
        var heroEquipment = new HeroEquipment(TestClasses.Soldier);

        var gun = TestableObjectFactory.Create<Equipment>().Initialized(TestClasses.Soldier);
        
        Assert.IsTrue(heroEquipment.CanEquip(gun));
    }
    
    [Test]
    public void HeroEquipment_EquipWeapon_IsEquipped()
    {
        var heroEquipment = new HeroEquipment(TestClasses.Soldier);
        var gun = TestableObjectFactory.Create<Equipment>().Initialized(TestClasses.Soldier);
        
        heroEquipment.Equip(gun);

        Assert.AreEqual(1, heroEquipment.All.Length);
    }
}
