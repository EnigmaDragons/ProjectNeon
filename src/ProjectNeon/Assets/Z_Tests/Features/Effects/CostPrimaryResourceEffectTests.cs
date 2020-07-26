using NUnit.Framework;
using UnityEngine;
using System.Linq;

public sealed class CostResourcesEffectTest
{
    private EffectData DamageTarget(float amount) => new EffectData
    {
        EffectType = EffectType.PhysicalDamage,
        FloatAmount = new FloatReference(amount), 
        EffectScope = new StringReference("Ammo")
    };

    [Test]
    public void CostResourcesEffect_ApplyEffect_ApplyWhenHaveResource()
    {
        Effect oneTimer = new CostResourceEffect(
            1,
            "Ammo"
        );
        Member perfromer = TestMembers.Create(
            s => {
                StatAddends addend = new StatAddends();
                addend.ResourceTypes = addend.ResourceTypes.Concat(
                    new InMemoryResourceType
                    {
                        Name = "Ammo",
                        StartingAmount = 2,
                        MaxAmount = 2
                    }
                ).ToArray();
                return addend;
            }
        );
        
        oneTimer.Apply(perfromer, new Single(perfromer));

        Assert.AreEqual(1, perfromer.State[new InMemoryResourceType { Name = "Ammo" }]);
    }
}
