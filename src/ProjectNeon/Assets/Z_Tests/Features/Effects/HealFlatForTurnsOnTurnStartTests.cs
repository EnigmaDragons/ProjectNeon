using NUnit.Framework;

public sealed class HealFlatForTurnsOnTurnStartTests
{
    [Test]
    public void Regeneration_ApplyEffect_RegenerateFlatForTurns()
    {
        Member attacker = TestMembers.With(StatType.Attack, 1);
        Member caster = TestMembers.Any();
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        //creating damage
        new Attack(5).Apply(attacker, new Single(target));

        new HealFlatForTurnsOnTurnStart(1, 3).Apply(caster, new Single(target));
        
        Message.Publish(new TurnEnd());
        Message.Publish(new TurnStart());
        Assert.AreEqual(
            6,
            target.State[TemporalStatType.HP],
            "Effect did not applied on turn 1."
        );

        Message.Publish(new TurnEnd());
        Message.Publish(new TurnStart());
        Assert.AreEqual(
            7,
            target.State[TemporalStatType.HP],
            "Effect did not applied on turn 2."
        );

        Message.Publish(new TurnEnd());
        Message.Publish(new TurnStart());
        Assert.AreEqual(
            8,
            target.State[TemporalStatType.HP],
            "Effect did not applied on turn 3."
        );

        Message.Publish(new TurnEnd());
        Message.Publish(new TurnStart());
        Assert.AreEqual(
            8,
            target.State[TemporalStatType.HP],
            "Effect applied on turn 4."
        );
    }
}
