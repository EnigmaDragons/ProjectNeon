using NUnit.Framework;

public sealed class DisabledTests
{
    [Test]
    public void Disable_DefaultMember_NoStun()
    {
        Assert.AreEqual(0, TestMembers.Any().State[TemporalStatType.Disabled]);
    }
    
    [Test]
    public void Disable_DisableFor5Turns_IsDisabledFor5Turns()
    {
        var e = new EffectData { EffectType = EffectType.DisableForTurns, NumberOfTurns = new IntReference(5) };
        var target = TestMembers.Any();

        TestEffects.Apply(e, TestMembers.Any(), target);
        
        Assert.AreEqual(5, target.State[TemporalStatType.Disabled]);
    }

    [Test]
    public void Disable_CleanseDebuffs_IsRemoved()
    {
        var e = new EffectData { EffectType = EffectType.DisableForTurns, NumberOfTurns = new IntReference(5) };
        var target = TestMembers.Any();
        
        TestEffects.Apply(e, TestMembers.Any(), target);
        TestEffects.Apply(new EffectData { EffectType = EffectType.RemoveDebuffs }, TestMembers.Any(), target);
        
        Assert.AreEqual(0, target.State[TemporalStatType.Disabled]);
    }

    [Test]
    public void Disable_AdvanceTurn_RemovesOneStunCounter()
    {
        var e = new EffectData { EffectType = EffectType.DisableForTurns, NumberOfTurns = new IntReference(5) };
        var target = TestMembers.Any();

        TestEffects.Apply(e, TestMembers.Any(), target);
        target.State.GetTurnEndEffects();
        
        Assert.AreEqual(4, target.State[TemporalStatType.Disabled]);
    }
}
