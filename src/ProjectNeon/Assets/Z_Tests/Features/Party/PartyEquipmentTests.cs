using System.Linq;
using NUnit.Framework;

public class PartyEquipmentTests
{
    [Test]
    public void PartyEquipmentCollection_DuplicateEquipment_EachCopyKept()
    {
        var collection = new PartyEquipmentCollection();
        
        var equip = new InMemoryEquipment().Initialized(TestClasses.Soldier);
        
        collection.Add(equip, equip);
        collection.MarkEquipped(equip);
        Assert.AreEqual(1, collection.Available.Count);
        collection.MarkEquipped(equip);
        Assert.AreEqual(0, collection.Available.Count);
        collection.MarkUnequipped(equip);
        Assert.AreEqual(1, collection.Available.Count);
        collection.MarkUnequipped(equip);
        Assert.AreEqual(2, collection.Available.Count);
    }
    
    [Test]
    public void PartyEquipmentCollection_DuplicateEquipment_CorrectAvailablesForClass()
    {
        var collection = new PartyEquipmentCollection();
        var soldier = TestClasses.Soldier;
        
        var equip = new InMemoryEquipment().Initialized(soldier);
        
        collection.Add(equip, equip);
        Assert.AreEqual(2, collection.AvailableFor(soldier).Count());
        collection.MarkEquipped(equip);
        Assert.AreEqual(1, collection.AvailableFor(soldier).Count());
        collection.MarkEquipped(equip);
        Assert.AreEqual(0, collection.AvailableFor(soldier).Count());
        collection.MarkUnequipped(equip);
        Assert.AreEqual(1, collection.AvailableFor(soldier).Count());
        collection.MarkUnequipped(equip);
        Assert.AreEqual(2, collection.AvailableFor(soldier).Count());
    }
}
