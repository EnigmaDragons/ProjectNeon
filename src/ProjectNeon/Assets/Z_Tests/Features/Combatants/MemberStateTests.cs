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
}
