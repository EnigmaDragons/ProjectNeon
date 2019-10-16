using Features.Combatants;
using NUnit.Framework;

public class StatsDecorationTests
{
    [Test]
    public void Stats_Barkskin_DecorateArmor()
    {
        var memberStats = new MemberState(new InMemoryStats());
        
        memberStats.ApplyUntilEndOfBattle(new BarkskinStats());
        
        Assert.AreEqual(10f, memberStats.Armor);
    }
}

