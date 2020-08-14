using System.Linq;
using NUnit.Framework;

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

        AllEffects.Apply(new EffectData
        {
            EffectType = EffectType.OnAttacked,
            NumberOfTurns = new IntReference(3),
            ReferencedEffectSequence = reactiveEffect
        }, target, target);
        
        var battleSnapshotBefore = new BattleStateSnapshot(target.GetSnapshot());
        var attackEffectData = new EffectData
        {
            EffectType = EffectType.Attack,
            FloatAmount = new FloatReference(1),
        };
        AllEffects.Apply(attackEffectData, attacker, target);
        var battleSnapshotAfter = new BattleStateSnapshot(target.GetSnapshot());
        
        var effectResolved = new EffectResolved(attackEffectData, attacker, new Single(target), battleSnapshotBefore, battleSnapshotAfter);

        var reactions = target.State.GetReactions(effectResolved);
        reactions.ForEach(r => r.Action.Actions
            .Where(a => a.Type == CardBattleActionType.Battle)
            .ForEach(be => AllEffects.Apply(be.BattleEffect, r.Source, r.Target)));
        
        Assert.AreEqual(1, target.State.Armor());
    }
}
