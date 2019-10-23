using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class StunTests
{

    private EffectData data = new EffectData { EffectType = EffectType.Stun, FloatAmount = new FloatReference(1)} ;

    private Member performer = new Member(
        1, 
        "Good Dummy One", 
        "Paladin", 
        TeamType.Party, 
        new StatAddends()
    );

    private Member target = new Member(
        2, 
        "Good Dummy Two", 
        "Confusable Dummy", 
        TeamType.Party, 
        new StatAddends()
    );

    [Test]
    public void Stun_ApplyEffect()
    {
        AllEffects.Apply(data, performer, new MemberAsTarget(target));
        Assert.AreEqual(
            true,
            target.State[Status.Stunned]
        );
    }
}
