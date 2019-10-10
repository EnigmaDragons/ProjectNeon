using System.Linq;
using NUnit.Framework;

public class StatsDecoRationTests
{
    [Test]
    public void Stats_Barkskin_DecorateArmor()
    {
        MemberState memberStats = TestableObjectFactory.Create<MemberState>();
        BattleStats spellStats = TestableObjectFactory.Create<BarkskinStats>();
        spellStats.Init(memberStats);
        memberStats.CurrentStats = spellStats;
        Assert.IsTrue(spellStats.Armor.Equals(10.0F));
    }
}

