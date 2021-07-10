using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

public class PartyCorpAffinityCalculatorTests
{
    private string Corp1 = "Corp1";
    private string Corp2 = "Corp2";

    [Test]
    public void PartyCorpAffinityCalculator_NoEquipment_NoAffinities()
    {
        var affinity = PartyCorpAffinityCalculator.ForEquippedEquipment(15, new List<Equipment>());
        
        Assert.IsTrue(affinity.All(x => x.Value == CorpAffinityStrength.None));
    }
    
    [Test]
    public void PartyCorpAffinityCalculator_OneEquipment_InterestedInCorp1()
    {
        var affinity = PartyCorpAffinityCalculator.ForEquippedEquipment(15, new List<Equipment> { new InMemoryEquipment { Corp = Corp1 }});
        
        Assert.AreEqual(CorpAffinityStrength.Interested, affinity[Corp1]);
    }
    
    [Test]
    public void PartyCorpAffinityCalculator_TwoEquipmentsOfDifferentCorp_InterestedInBothCorps()
    {
        var affinity = PartyCorpAffinityCalculator.ForEquippedEquipment(15, new List<Equipment>
        {
            new InMemoryEquipment { Corp = Corp1 },
            new InMemoryEquipment { Corp = Corp2 },
        });
        
        Assert.AreEqual(CorpAffinityStrength.Interested, affinity[Corp1]);
        Assert.AreEqual(CorpAffinityStrength.Interested, affinity[Corp2]);
    }
    
    [Test]
    public void PartyCorpAffinityCalculator_OneFifthSlotsFilledWithOneCorps_LoyalToCorp()
    {
        var affinity = PartyCorpAffinityCalculator.ForEquippedEquipment(15, new List<Equipment>
        {
            new InMemoryEquipment { Corp = Corp1 },
            new InMemoryEquipment { Corp = Corp1 },
            new InMemoryEquipment { Corp = Corp1 }
        });
        
        Assert.AreEqual(CorpAffinityStrength.Loyal, affinity[Corp1]);
    }
    
    [Test]
    public void PartyCorpAffinityCalculator_OneFifthSlotsFilledWithOneCorpsButHasSameAmountOfOtherCorp_InterestedInCorps()
    {
        var affinity = PartyCorpAffinityCalculator.ForEquippedEquipment(15, new List<Equipment>
        {
            new InMemoryEquipment { Corp = Corp1 },
            new InMemoryEquipment { Corp = Corp1 },
            new InMemoryEquipment { Corp = Corp1 },
            new InMemoryEquipment { Corp = Corp2 },
            new InMemoryEquipment { Corp = Corp2 },
            new InMemoryEquipment { Corp = Corp2 }
        });
        
        Assert.AreEqual(CorpAffinityStrength.Interested, affinity[Corp1]);
        Assert.AreEqual(CorpAffinityStrength.Interested, affinity[Corp2]);
    }
    
    [Test]
    public void PartyCorpAffinityCalculator_OneThirdSlotsFilledWithOneCorps_FanaticForCorp()
    {
        var affinity = PartyCorpAffinityCalculator.ForEquippedEquipment(15, new List<Equipment>
        {
            new InMemoryEquipment { Corp = Corp1 },
            new InMemoryEquipment { Corp = Corp1 },
            new InMemoryEquipment { Corp = Corp1 },
            new InMemoryEquipment { Corp = Corp1 },
            new InMemoryEquipment { Corp = Corp1 }
        });
        
        Assert.AreEqual(CorpAffinityStrength.Fanatic, affinity[Corp1]);
    }
    
    [Test]
    public void PartyCorpAffinityCalculator_OneThirdSlotsFilledWithOneCorpsButHasSameAmountOfOtherCorp_InterestedInCorps()
    {
        var affinity = PartyCorpAffinityCalculator.ForEquippedEquipment(15, new List<Equipment>
        {
            new InMemoryEquipment { Corp = Corp1 },
            new InMemoryEquipment { Corp = Corp1 },
            new InMemoryEquipment { Corp = Corp1 },
            new InMemoryEquipment { Corp = Corp1 },
            new InMemoryEquipment { Corp = Corp1 },
            new InMemoryEquipment { Corp = Corp2 },
            new InMemoryEquipment { Corp = Corp2 },
            new InMemoryEquipment { Corp = Corp2 },
            new InMemoryEquipment { Corp = Corp2 },
            new InMemoryEquipment { Corp = Corp2 }
        });
        
        Assert.AreEqual(CorpAffinityStrength.Interested, affinity[Corp1]);
        Assert.AreEqual(CorpAffinityStrength.Interested, affinity[Corp2]);
    }
}
