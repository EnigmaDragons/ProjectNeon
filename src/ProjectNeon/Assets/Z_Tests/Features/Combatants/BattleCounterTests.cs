using NUnit.Framework;

[TestFixture]
public class BattleCounterTests
{
    [Test]
    public void BattleCounter_AddANegativeFractionalAmount_RoundsDownInsteadOfUp()
    {
        var counter = new BattleCounter(TemporalStatType.HP, 10, () => 20);
        
        counter.ChangeBy(-0.66f);
        
        Assert.AreEqual(9, counter.Amount);
    }
}