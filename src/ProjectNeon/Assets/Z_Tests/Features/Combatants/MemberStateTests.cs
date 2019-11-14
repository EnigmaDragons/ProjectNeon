using NUnit.Framework;

[TestFixture]
public sealed class MemberStateTests
{
    [Test]
    public void MemberState_GainPrimaryResourceAmount_IsCorrect()
    {
        var resource = new InMemoryResourceType {Name = "SampleResource", MaxAmount = 3};
        var member = new MemberState(new StatAddends { ResourceTypes = resource.AsArray() });
        
        member.GainPrimaryResource(2);
        
        Assert.AreEqual(member[resource], 2);
    }

    [Test]
    public void MemberState_Resource_StartsWithCorrectNumber()
    {
        var resource = new InMemoryResourceType { Name = "SampleResource", StartingAmount = 8};
        var member = new MemberState(new StatAddends { ResourceTypes = resource.AsArray() });

        Assert.AreEqual(8, member[resource]);
    }
}
