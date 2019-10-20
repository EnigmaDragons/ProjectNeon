using System;
using System.Linq;
using NUnit.Framework;

public sealed class EffectCreationTests
{
    [Test]
    public void Effects_CanCreateAll()
    {
        var types = Enum.GetValues(typeof(EffectType)).Cast<EffectType>().Select(t => new EffectData {EffectType = t});

        foreach (var effectData in types)
            Assert.IsNotNull(AllEffects.Create(effectData), $"Could not create Effect of Type {effectData.EffectType.ToString()}");
    }
}
