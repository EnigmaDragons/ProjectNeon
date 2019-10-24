using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class StunTests
{
    private EffectData data = new EffectData { EffectType = EffectType.Stun, NumberOfTurns = new IntReference(1)} ;

    private Member target = new Member(
        1, 
        "Good Dummy Two", 
        "Confusable Dummy", 
        TeamType.Party, 
        new StatAddends()
    );

    [Test]
    public void Stun_ApplyEffect()
    {
        AllEffects.Apply(data, target, new MemberAsTarget(target));
        Assert.AreEqual(
            true,
            target.State[TemporalStatType.Stun] > 0
        );
    }
}
