using Features.CombatantStates.Reactions;
using NUnit.Framework;

public class OnShieldBrokenTests
{
    [TestCase(0, false)]
    [TestCase(1, true)]
    [TestCase(2, false)]
    public void OnShieldBroken_Attacked_TriggersCorrectly(int startingShields, bool triggered)
    {
        var target = TestMembers.Create(x => x.With(StatType.MaxHP, 10).With(StatType.Toughness, 1));
        var attacker = TestMembers.Create(s => s.With(StatType.Attack, 1));
        target.State.AdjustShield(startingShields);

        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.ReactWithCard,
            NumberOfTurns = new IntReference(3),
            FloatAmount = new FloatReference(-1),
            ReactionConditionType = ReactionConditionType.OnShieldBroken,
            ReactionSequence = TestCards.ReactionCard(
                ReactiveMember.Possessor,
                ReactiveTargetScope.Self,
                new EffectData { 
                    EffectType = EffectType.AdjustStatAdditivelyFormula, 
                    Formula = "1", 
                    EffectScope = new StringReference("Armor"), 
                    NumberOfTurns = new IntReference(-1) 
                })
        }, target, target);

        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.Attack,
            FloatAmount = new FloatReference(1),
            EffectScope = new StringReference(ReactiveTargetScope.Self.ToString())
        }, attacker, target);

        Assert.AreEqual(triggered ? 1 : 0, target.State.Armor());
    }
}
