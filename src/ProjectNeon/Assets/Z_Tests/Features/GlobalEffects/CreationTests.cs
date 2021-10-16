using System;
using System.Linq;
using NUnit.Framework;

public class CreationTests
{
    [Test]
    public void Effects_CanCreateAll()
    {
        var types = Enum.GetValues(typeof(GlobalEffectType)).Cast<GlobalEffectType>().Select(t => new GlobalEffectData {EffectType = t});

        foreach (var effectData in types)
        {
            try
            {
                Assert.IsNotNull(AllGlobalEffects.Create(effectData), $"Could not create Global Effect of Type '{effectData.EffectType.ToString()}'");
            }
            catch (Exception e)
            {
                Assert.Fail($"Could not create Global Effect of Type '{effectData.EffectType.ToString()}' - {e.Message}");
            }
        }
    }
}
