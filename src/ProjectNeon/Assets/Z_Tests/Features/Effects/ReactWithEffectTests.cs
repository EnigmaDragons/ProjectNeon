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
                        EffectScope = new StringReference(TemporalStatType.Aegis.ToString()), 
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
        Assert.AreEqual(1, target.State[TemporalStatType.Aegis], "Reaction didn't trigger");
    }

    [Test]
    public void ReactWithEffect_OnVulnerable_Works()
    {
        var attacker = TestMembers.Named("Robot");
        var possessor = TestMembers.Create(s => s.With(StatType.Toughness, 5).With(StatType.MaxShield, 10));
        
        TestEffects.Apply(Gain5ShieldOnReactionCondition(ReactionConditionType.OnVulnerable), possessor, possessor);
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.ApplyVulnerable,
            FloatAmount = new FloatReference(1)
        }, attacker, possessor);
        
        Assert.AreEqual(5, possessor.CurrentShield());
    }
    
    [Test]
    public void ReactWithEffect_OnDodged_Works()
    {
        var attacker = TestMembers.Any();
        var possessor = TestMembers.Create(s => s.With(StatType.Toughness, 5).With(StatType.MaxShield, 10));
        possessor.Apply(m => m.Adjust(TemporalStatType.Dodge, 1));
        TestEffects.Apply(Gain5ShieldOnReactionCondition(ReactionConditionType.OnDodged), possessor, possessor);

        TestEffects.Apply(TestEffects.BasicAttack, attacker, possessor);
        
        Assert.AreEqual(5, possessor.CurrentShield());
    }
    
    [Test]
    public void ReactWithEffect_OnAegised_Works()
    {
        var attacker = TestMembers.Any();
        var possessor = TestMembers.Create(s => s.With(StatType.Toughness, 5).With(StatType.MaxShield, 10));
        possessor.Apply(m => m.Adjust(TemporalStatType.Aegis, 1));
        TestEffects.Apply(Gain5ShieldOnReactionCondition(ReactionConditionType.OnAegised), possessor, possessor);

        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.ApplyVulnerable,
            NumberOfTurns = new IntReference(1)
        }, attacker, possessor);
        
        Assert.AreEqual(5, possessor.CurrentShield());
    }

    private static EffectData Gain5ShieldOnReactionCondition(ReactionConditionType condition)
        => new EffectData
        {
            EffectType = EffectType.ReactWithEffect,
            ReactionConditionType = condition,
            FloatAmount = new FloatReference(-1),
            NumberOfTurns = new IntReference(-1),
            ReactionEffect = Gain5Shield
        };
    
    private static CardReactionSequence Gain5Shield => TestCards.ReactionEffect(
        ReactiveMember.Possessor,
        ReactiveTargetScope.Possessor,
        new EffectData
        {
            EffectType = EffectType.ShieldFormula,
            Formula = "1 * Toughness"
        });
}
