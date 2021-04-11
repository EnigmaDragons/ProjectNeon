using NUnit.Framework;

public class PreventDeathCounterTests
{
    [Test]
    public void Member_WithPreventDeath_CantDie()
    {
        var member = TestMembers.Create(s => s.With(StatType.MaxHP, 2).With(TemporalStatType.PreventDeath, 1));

        member.State.TakeDamage(300);
        member.State.TakeRawDamage(300);
        
        Assert.AreEqual(1, member.CurrentHp());
        Assert.True(member.IsConscious());
    }
}