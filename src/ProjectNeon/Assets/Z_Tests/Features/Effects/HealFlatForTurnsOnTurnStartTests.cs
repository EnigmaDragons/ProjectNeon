using NUnit.Framework;

public sealed class HealFlatForTurnsOnTurnStartTests
{
    [Ignore("Pending Rework of EffectOnTurnStart")]
    [Test]
    public void Regeneration_ApplyEffect_RegenerateFlatForTurns()
    {
        var attacker = TestMembers.With(StatType.Attack, 1);
        var caster = TestMembers.Any();
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        new Attack(5).Apply(attacker, new Single(target));

        new HealOverTime(1, 3).Apply(caster, new Single(target));
        
        target.State.GetTurnStartEffects();
        target.State.GetTurnEndEffects();
        Assert.AreEqual(
            6,
            target.State[TemporalStatType.HP],
            "Effect did not applied on turn 1."
        );

        target.State.GetTurnStartEffects();
        target.State.GetTurnEndEffects();
        Assert.AreEqual(
            7,
            target.State[TemporalStatType.HP],
            "Effect did not applied on turn 2."
        );

        target.State.GetTurnStartEffects();
        target.State.GetTurnEndEffects();
        Assert.AreEqual(
            8,
            target.State[TemporalStatType.HP],
            "Effect did not applied on turn 3."
        );

        target.State.GetTurnStartEffects();
        target.State.GetTurnEndEffects();
        Assert.AreEqual(
            8,
            target.State[TemporalStatType.HP],
            "Effect applied on turn 4."
        );
    }
}
