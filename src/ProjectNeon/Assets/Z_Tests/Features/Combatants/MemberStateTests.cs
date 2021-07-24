using NUnit.Framework;

[TestFixture]
public sealed class MemberStateTests
{
    [Test]
    public void MemberState_GainPrimaryResourceAmount_IsCorrect()
    {
        var resource = new InMemoryResourceType {Name = "SampleResource", MaxAmount = 3};
        var member = new MemberState(1, "John", new StatAddends { ResourceTypes = resource.AsArray() }, StatType.Attack);
        
        member.AdjustPrimaryResource(2);
        
        Assert.AreEqual(member[resource], 2);
    }

    [Test]
    public void MemberState_Resource_StartsWithCorrectNumber()
    {
        var resource = new InMemoryResourceType { Name = "SampleResource", StartingAmount = 8};
        var member = new MemberState(1, "John", new StatAddends { ResourceTypes = resource.AsArray() }, StatType.Attack);

        Assert.AreEqual(8, member[resource]);
    }

    [Test]
    public void MemberState_WhenHasTauntAndGainsStealth_LosesAllTaunt()
    {
        var m = TestMembers.Any().State;
        m.Adjust(TemporalStatType.Taunt, 1);

        m.Adjust(TemporalStatType.Stealth, 1);
        
        Assert.AreEqual(1, m[TemporalStatType.Stealth], "Didn't Have Expected Stealth");
        Assert.AreEqual(0, m[TemporalStatType.Taunt], "Had Taunt After Stealthing");
    }
    
    [Test]
    public void MemberState_WhenHasStealthAndGainsTaunt_LosesAllStealthAndGainsProminent()
    {
        var m = TestMembers.Any().State;
        m.Adjust(TemporalStatType.Stealth, 1);
        
        m.Adjust(TemporalStatType.Taunt, 1);

        Assert.AreEqual(1, m[TemporalStatType.Taunt], "Didn't Have Expected Taunt");
        Assert.AreEqual(1, m[TemporalStatType.Prominent], "Didn't Have Expected Taunt");
        Assert.AreEqual(0, m[TemporalStatType.Stealth], "Had Stealth after Taunting");
    }
}
