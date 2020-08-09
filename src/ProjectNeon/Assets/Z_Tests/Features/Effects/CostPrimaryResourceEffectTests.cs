using NUnit.Framework;
using System.Linq;

public sealed class CostResourcesEffectTest
{
    [Test]
    public void CostResourcesEffect_ApplyEffect_ApplyWhenHaveResource()
    {
        var effect = new CostResourceEffect(1, "Ammo");
        var member = TestMembers.Create(
            s => {
                s.ResourceTypes = s.ResourceTypes.Concat(
                    new InMemoryResourceType
                    {
                        Name = "Ammo",
                        StartingAmount = 2,
                        MaxAmount = 2
                    }
                ).ToArray();
                return s;
            }
        );
        
        effect.Apply(member, new Single(member));

        Assert.AreEqual(1, member.State[new InMemoryResourceType { Name = "Ammo" }]);
    }
}
