using NUnit.Framework;

public sealed class ReactiveTriggerTests
{
    [Test]
    public void ReactiveTrigger_OnAttacked_GainedOneArmor()
    {
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10));
        var attacker = TestMembers.Create(s => s.With(StatType.Attack, 1));

        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.ReactWithCard,
            ReactionConditionType = ReactionConditionType.OnAttacked,
            NumberOfTurns = new IntReference(3),
            FloatAmount = new FloatReference(-1),
            ReactionSequence = TestCards.ReactionCard(
                ReactiveMember.Possessor, 
                ReactiveTargetScope.Self, 
                new EffectData
                {
                    EffectType = EffectType.AdjustStatAdditivelyFormula, 
                    Formula = "1", 
                    EffectScope = new StringReference("Armor"), 
                    NumberOfTurns = new IntReference(-1)
                })
        }, target, target);
        
        TestEffects.Apply(new EffectData
        {            
            EffectType = EffectType.AttackFormula,
            Formula = "1 * Attack"
        }, attacker, target);
        
        Assert.AreEqual(1, target.State.Armor());
    }

    [Test]
    public void ReactiveTrigger_OnAttacked_AttackerHitForOneDamage()
    {
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Attack, 1));
        var attacker = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Attack, 1));

        var reactionCardType = TestCards.ReactionCard(
            ReactiveMember.Originator, 
            ReactiveTargetScope.Source, 
            new EffectData {
                EffectType = EffectType.AttackFormula,
                Formula = "1 * Attack"
            });

        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.ReactWithCard,
            ReactionConditionType = ReactionConditionType.OnAttacked,
            NumberOfTurns = new IntReference(3),
            FloatAmount = new FloatReference(-1),
            ReactionSequence = reactionCardType
        }, target, target);
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.AttackFormula,
            Formula = "1 * Attack",
            EffectScope = new StringReference(ReactiveTargetScope.Self.ToString())
        }, attacker, target);
        
        Assert.AreEqual(9, attacker.CurrentHp());
    }
}
