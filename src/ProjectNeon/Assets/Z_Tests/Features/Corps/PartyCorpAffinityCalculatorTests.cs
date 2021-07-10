using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

public class PartyCorpAffinityCalculatorTests
{
    private static InMemoryCorp RivalToCorp1 = new InMemoryCorp { Name = "RivalToCorp1", RivalCorpNames = "Corp1".AsArray() };
    private static InMemoryCorp Corp1 = new InMemoryCorp { Name = "Corp1", RivalCorpNames = RivalToCorp1.Name.AsArray() };
    private static InMemoryCorp Corp2 = new InMemoryCorp { Name = "Corp2" };
    private static InMemoryCorp Corp3 = new InMemoryCorp { Name = "Corp3" };

    private readonly Dictionary<string, Corp> _allCorps = new Dictionary<string, Corp>
    {
        { Corp1.Name, Corp1 },
        { Corp2.Name, Corp2 },
        { Corp3.Name, Corp3 },
        { RivalToCorp1.Name, RivalToCorp1 }
    };

    [Test]
    public void PartyCorpAffinityCalculator_NoEquipment_NoAffinities()
    {
        var affinity = GetAffinity(new List<Equipment>());
        
        Assert.IsTrue(affinity.All(x => x.Value == CorpAffinityStrength.None));
    }
    
    [Test]
    public void PartyCorpAffinityCalculator_OneEquipment_InterestedInCorp1()
    {
        var affinity = GetAffinity(new List<Equipment>
        {
            new InMemoryEquipment { Corp = Corp1 }
        });
        
        Assert.AreEqual(CorpAffinityStrength.Interested, affinity[Corp1]);
    }
    
    [Test]
    public void PartyCorpAffinityCalculator_TwoEquipmentsOfDifferentCorp_InterestedInBothCorps()
    {
        var affinity = GetAffinity(new List<Equipment>
        {
            new InMemoryEquipment { Corp = Corp1 },
            new InMemoryEquipment { Corp = Corp2 },
        });
        
        Assert.AreEqual(CorpAffinityStrength.Interested, affinity[Corp1]);
        Assert.AreEqual(CorpAffinityStrength.Interested, affinity[Corp2]);
    }
    
    [Test]
    public void PartyCorpAffinityCalculator_ThreeEquipmentsOfAllDifferentCorps_NotInterestedInAnyCorps()
    {
        var affinity = GetAffinity(new List<Equipment>
        {
            new InMemoryEquipment { Corp = Corp1 },
            new InMemoryEquipment { Corp = Corp2 },
            new InMemoryEquipment { Corp = Corp3 },
        });
        
        Assert.IsTrue(affinity.All(x => x.Value == CorpAffinityStrength.None));
    }
    
    [Test]
    public void PartyCorpAffinityCalculator_TwoEquipmentsOfOneCorpWithStrayGearFromOthers_OnlyInterestedInCorp1()
    {
        var affinity = GetAffinity(new List<Equipment>
        {
            new InMemoryEquipment { Corp = Corp1 },
            new InMemoryEquipment { Corp = Corp1 },
            new InMemoryEquipment { Corp = Corp2 },
            new InMemoryEquipment { Corp = Corp3 },
        });
        
        Assert.AreEqual(CorpAffinityStrength.Interested, affinity[Corp1]);
        Assert.AreEqual(CorpAffinityStrength.None, affinity[Corp2]);
        Assert.AreEqual(CorpAffinityStrength.None, affinity[Corp3]);
    }
    
    [Test]
    public void PartyCorpAffinityCalculator_OneFifthSlotsFilledWithOneCorps_LoyalToCorp()
    {
        var affinity = GetAffinity(new List<Equipment>
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
        var affinity = GetAffinity(new List<Equipment>
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
        var affinity = GetAffinity(new List<Equipment>
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
        var affinity = GetAffinity(new List<Equipment>
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
    
    [Test]
    public void PartyCorpAffinityCalculator_EvenAmountOfGearWithRival_NoAffinityForEitherRival()
    {
        var affinity = GetAffinity(new List<Equipment>
        {
            new InMemoryEquipment { Corp = Corp1 },
            new InMemoryEquipment { Corp = RivalToCorp1 },
        });
        
        Assert.AreEqual(CorpAffinityStrength.None, affinity[Corp1]);
        Assert.AreEqual(CorpAffinityStrength.None, affinity[RivalToCorp1]);
    }

    [Test]
    public void PartyCorpAffinityCalculator_LoyalForRivalCorp_AffinityLevelRivalToCorp()
    {
        var affinity = GetAffinity(new List<Equipment>
        {
            new InMemoryEquipment { Corp = RivalToCorp1 },
            new InMemoryEquipment { Corp = RivalToCorp1 },
            new InMemoryEquipment { Corp = RivalToCorp1 }
        });
        
        Assert.AreEqual(CorpAffinityStrength.Rival, affinity[Corp1]);
    }
    
    [Test]
    public void PartyCorpAffinityCalculator_FanaticForRivalCorp_AffinityLevelDetrifactorToCorp()
    {
        var affinity = GetAffinity(new List<Equipment>
        {
            new InMemoryEquipment { Corp = RivalToCorp1 },
            new InMemoryEquipment { Corp = RivalToCorp1 }, 
            new InMemoryEquipment { Corp = RivalToCorp1 },
            new InMemoryEquipment { Corp = RivalToCorp1 },
            new InMemoryEquipment { Corp = RivalToCorp1 }
        });
        
        Assert.AreEqual(CorpAffinityStrength.Detrifactor, affinity[Corp1]);
    }
    
    private PartyCorpAffinity GetAffinity(List<Equipment> equipped) =>
        PartyCorpAffinityCalculator.ForEquippedEquipment(15, _allCorps, equipped);
}
