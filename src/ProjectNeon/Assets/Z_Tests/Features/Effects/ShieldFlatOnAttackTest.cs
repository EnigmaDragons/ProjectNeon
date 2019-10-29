using NUnit.Framework;
using UnityEngine;

public sealed class TriggeredEffectTest
{
    private EffectData ChangeShieldOnAttackBy(float amount) => new EffectData { EffectType = EffectType.ShieldFlatOnAttack, FloatAmount = new FloatReference(amount) };

    [Test]
    public void TriggeredEffect_TriggerEffect_EffectIsTriggeredOnEvent()
    {
        var addShieldEffectTriggered = ChangeShieldOnAttackBy(5);
        var performer = TestMembers.Any();
        var target = TestMembers.Any();
        //Applying the effect (preparing it)
        AllEffects.Apply(addShieldEffectTriggered, performer, new MemberAsTarget(target));
        //Triggering events
        AttackEvent evt = (AttackEvent)ScriptableObject.CreateInstance("AttackEvent");
        evt.Publish();
        Assert.AreEqual(5, performer.State[TemporalStatType.Shield]);
    }
}
