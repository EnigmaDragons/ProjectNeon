using NUnit.Framework;
using System.Linq;

[TestFixture]
public sealed class HealPrimaryResourceTests
{
    [Test]
    public void HealPrimaryResourceTests_ApplyEffect()
    {
        EffectData healPrimary = new EffectData { EffectType = EffectType.HealPrimaryResource };
        Member performer = TestMembers.Create(
            s =>
            {
                StatAddends addend = new StatAddends();
                addend.ResourceTypes = addend.ResourceTypes.Concat(
                    new InMemoryResourceType
                    {
                        Name = "Resource",
                        StartingAmount = 0,
                        MaxAmount = 2
                    }
                ).ToArray();
                addend.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f);
                return addend;
            }
        );
        performer.State.TakeRawDamage(6);
        performer.State.GainPrimaryResource(2);
        
        TestEffects.Apply(healPrimary, performer, new Single(performer));
        Assert.AreEqual(
            6, 
            performer.State[TemporalStatType.HP],
            "Did not healed primary resource quantity"
        );
    }
}
