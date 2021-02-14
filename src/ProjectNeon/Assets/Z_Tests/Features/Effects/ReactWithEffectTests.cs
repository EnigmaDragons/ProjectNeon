using Features.CombatantStates.Reactions;
using NUnit.Framework;

public class ReactWithEffectTests
{
    [Test]
    public void ReactWithEffect_OnStunned_Works()
    {
        var attacker = TestMembers.Named("Electroblade");
        var target = TestMembers.Named("Robot");

        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.ReactWithEffect,
            ReactionConditionType = ReactionConditionType.OnCausedStun,
            FloatAmount = new FloatReference(-1),
            NumberOfTurns = new IntReference(-1),
            ReactionEffect = TestCards.ReactionEffect(
                    ReactiveMember.Possessor, 
                    ReactiveTargetScope.Target, 
                    new EffectData
                    {
                        EffectType = EffectType.AdjustCounterFormula, 
                        EffectScope = new StringReference("Evade"), 
                        Formula = "1"
                    })
        }, attacker, attacker);
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.AdjustCounterFormula,
            EffectScope = new StringReference(TemporalStatType.CardStun.ToString()),
            Formula = "1"
        }, attacker, target);
        
        Assert.AreEqual(1, target.State[TemporalStatType.CardStun], "Card Stun wasn't applied");
        Assert.AreEqual(1, target.State[TemporalStatType.Evade], "Reaction didn't trigger");
    }

    [Test]
    public void ReactWithEffect_OnVulnerable_Works()
    {
        var attacker = TestMembers.Named("Robot");
        var possessor = TestMembers.Create(s => s.With(StatType.Toughness, 5).With(StatType.MaxShield, 10));
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.ReactWithEffect,
            ReactionConditionType = ReactionConditionType.OnVulnerable,
            FloatAmount = new FloatReference(-1),
            NumberOfTurns = new IntReference(-1),
            ReactionEffect = TestCards.ReactionEffect(
                ReactiveMember.Possessor, 
                ReactiveTargetScope.Possessor, 
                new EffectData
                {
                    EffectType = EffectType.ShieldFormula,
                    Formula = "1 * Toughness"
                })
        }, possessor, possessor);
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.ApplyVulnerable,
            FloatAmount = new FloatReference(1)
        }, attacker, possessor);
        
        Assert.AreEqual(5, possessor.CurrentShield());
    }
}
