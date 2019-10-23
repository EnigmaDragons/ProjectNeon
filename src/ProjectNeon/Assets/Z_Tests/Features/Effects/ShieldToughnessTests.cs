using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ShieldToughnessTests
{

    private EffectData data = new EffectData { EffectType = EffectType.ShieldToughness, FloatAmount = new FloatReference(1)} ;

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
        "Wooden Dummy", 
        TeamType.Party, 
        new StatAddends().With(StatType.Toughness, 5) 
    );

    [Test]
    public void ShieldToughness_ApplyEffect()
    {
        Debug.Log("Testtinf");
        AllEffects.Apply(data, performer, new MemberAsTarget(target));
        Assert.AreEqual(
            5,
            target.State[TemporalStatType.Shield]
        );
    }
}
