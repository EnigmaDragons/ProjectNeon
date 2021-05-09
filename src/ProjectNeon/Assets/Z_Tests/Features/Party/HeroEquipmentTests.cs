using NUnit.Framework;
using UnityEditor.Graphs;

public class HeroEquipmentTests
{
    [Test]
    public void HeroEquipment_New_NoEquipment()
    {
        var heroEquipment = new HeroEquipment();
        
        Assert.AreEqual(0, heroEquipment.All.Length);
    }
    
    [Test]
    public void HeroEquipment_CanEquipForNonMatchingArchetype_IsFalse()
    {
        var heroEquipment = new HeroEquipment("soldier");

        var sword = new InMemoryEquipment().Initialized("paladin");
        
        Assert.IsFalse(heroEquipment.CanEquip(sword));
    }
    
    [Test]
    public void HeroEquipment_CanEquipForMatchingArchetype_IsTrue()
    {
        var heroEquipment = new HeroEquipment("soldier");

        var gun = new InMemoryEquipment().Initialized("soldier");
        
        Assert.IsTrue(heroEquipment.CanEquip(gun));
    }
    
    [Test]
    public void HeroEquipment_EquipWeapon_IsEquipped()
    {
        var heroEquipment = new HeroEquipment("soldier");
        var gun = new InMemoryEquipment().Initialized("soldier");
        
        heroEquipment.Equip(gun);

        Assert.AreEqual(1, heroEquipment.All.Length);
    }

    [Test]
    public void HeroEquipment_UnequipAugmentWhenHasTwoCopies_StillHasOneEquipped()
    {
        var heroEquipment = new HeroEquipment("soldier");
        var augment = new InMemoryEquipment { Slot = EquipmentSlot.Augmentation };
        
        heroEquipment.Equip(augment);
        heroEquipment.Equip(augment);
        heroEquipment.Unequip(augment);
        
        Assert.AreEqual(1, heroEquipment.All.Length);
    }
}
