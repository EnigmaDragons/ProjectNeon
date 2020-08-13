using NUnit.Framework;

public sealed class TurnStunTests
{
    [Test]
    public void TurnStun_DefaultMember_NoStun()
    {
        Assert.AreEqual(0, TestMembers.Any().State[TemporalStatType.TurnStun]);
    }
    
    [Test]
    public void TurnStun_StunFor5Turns_IsStunnedFor5Turns()
    {
        var stunForTurns = new EffectData { EffectType = EffectType.StunForTurns, NumberOfTurns = new IntReference(5) };
        var target = TestMembers.Any();

        AllEffects.Apply(stunForTurns, TestMembers.Any(), target);
        
        Assert.AreEqual(5, target.State[TemporalStatType.TurnStun]);
    }

    [Test]
    public void Stun_CleanseDebuffs_IsRemoved()
    {
        var stunForTurns = new EffectData { EffectType = EffectType.StunForTurns, NumberOfTurns = new IntReference(5) };
        var target = TestMembers.Any();
        
        AllEffects.Apply(stunForTurns, TestMembers.Any(), target);
        AllEffects.Apply(new EffectData { EffectType = EffectType.RemoveDebuffs }, TestMembers.Any(), target);
        
        Assert.AreEqual(0, target.State[TemporalStatType.TurnStun]);
    }

    [Test]
    public void Stun_AdvanceTurn_RemovesOneStunCounter()
    {
        var stunForTurns = new EffectData { EffectType = EffectType.StunForTurns, NumberOfTurns = new IntReference(5) };
        var target = TestMembers.Any();

        AllEffects.Apply(stunForTurns, TestMembers.Any(), target);
        target.State.OnTurnEnd();
        
        Assert.AreEqual(4, target.State[TemporalStatType.TurnStun]);
    }
}
