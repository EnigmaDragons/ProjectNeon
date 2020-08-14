using NUnit.Framework;

[Ignore("WIP")]
public sealed class ReactiveTriggerTests
{
    [Test]
    public void ReactiveTrigger_OnLostHp_GainedOneArmor()
    {
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10));
        var attacker = TestMembers.Create(s => s.With(StatType.Attack, 1));

        var reactiveEffect = TestableObjectFactory.Create<CardActionsData>();
        reactiveEffect.Actions = new [] { new CardActionV2(new EffectData
            {
                EffectType = EffectType.ArmorFlat,
                FloatAmount = new FloatReference(1)
            }) 
        };
        
        var reactions = AllEffects.ApplyAndGetReactiveEffects(new EffectData
        {
            EffectType = EffectType.OnAttacked,
            NumberOfTurns = new IntReference(3),
            ReferencedEffectSequence = reactiveEffect
        }, attacker, target);
        
        reactions.ForEach(r => r.Execute());
        
        Assert.AreEqual(1, target.State.Armor());
    }
}
