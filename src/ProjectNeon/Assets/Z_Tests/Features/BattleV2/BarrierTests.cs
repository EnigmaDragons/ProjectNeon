using NUnit.Framework;

[TestFixture]
public class BarrierTests
{
    private static readonly InMemoryCard BasicAttackCard = new InMemoryCard
    {
        ActionSequences = new[] { 
            CardActionSequence.Create(Scope.One, Group.Opponent, AvoidanceType.Evade, 
                TestableObjectFactory.Create<CardActionsData>().Initialized(new CardActionV2(TestEffects.BasicAttack)), repeatX: false)
        }
    };
    
    [Test]
    public void AttackerIsBlindAndDefenderHasBarrier_BarrierStillPresent()
    {
        var defender = TestMembers.Any();
        defender.Apply(m => m.Adjust(TemporalStatType.Barrier, 1));

        var attacker = TestMembers.Any();
        attacker.Apply(m => m.Adjust(TemporalStatType.Blind, 1));

        var basicAttackCard = new Card(0, attacker, BasicAttackCard);
        basicAttackCard.Execute(new Target[] { new Single(defender),}, ResourceQuantity.None, attacker, defender);
        
        Assert.AreEqual(1, defender.State[TemporalStatType.Barrier]);
    }
    
    [Test]
    public void AttackerPlaysAttack_BarrierGoneAndHpUnchanged()
    {
        var defender = TestMembers.Any();
        defender.Apply(m => m.Adjust(TemporalStatType.Barrier, 1));

        var attacker = TestMembers.Any();

        var basicAttackCard = new Card(0, attacker, BasicAttackCard);
        basicAttackCard.Execute(new Target[] { new Single(defender),}, ResourceQuantity.None, attacker, defender);
        
        Assert.AreEqual(0, defender.State[TemporalStatType.Barrier], "Barrier Not Consumed");
        Assert.AreEqual(0, defender.State.MissingHp());
    }
    
    [Test]
    public void AttackerPlaysDoubleAttack_TwoBarriersGoneAndHpUnchanged()
    {
        var defender = TestMembers.Any();
        defender.Apply(m => m.Adjust(TemporalStatType.Barrier, 2));

        var attacker = TestMembers.Any();

        var basicAttackCard = new Card(0, attacker, new InMemoryCard
        {
            ActionSequences = new[] { 
                CardActionSequence.Create(Scope.One, Group.Opponent, AvoidanceType.NotSpecified, 
                    TestableObjectFactory.Create<CardActionsData>().Initialized(new CardActionV2(TestEffects.BasicAttack)), repeatX: false),
                CardActionSequence.Create(Scope.One, Group.Opponent, AvoidanceType.NotSpecified, 
                    TestableObjectFactory.Create<CardActionsData>().Initialized(new CardActionV2(TestEffects.BasicAttack)), repeatX: false)
            }
        });
        basicAttackCard.Execute(new Target[] { new Single(defender), new Single(defender)}, ResourceQuantity.None, attacker, defender);
        
        Assert.AreEqual(0, defender.State[TemporalStatType.Barrier], "Barrier Not Consumed");
        Assert.AreEqual(0, defender.State.MissingHp());
    }
    
    [Ignore("Test Engine Doesn't Resolve Preventions Yes")]
    [Test]
    public void AttackerIsBlindAndDefenderHasBarrier_BlindGone()
    {
        var defender = TestMembers.Any();
        defender.Apply(m => m.Adjust(TemporalStatType.Barrier, 1));

        var attacker = TestMembers.Any();
        attacker.Apply(m => m.Adjust(TemporalStatType.Blind, 1));

        var basicAttackCard = new Card(0, attacker, BasicAttackCard);
        basicAttackCard.Execute(new Target[] { new Single(defender),}, ResourceQuantity.None, attacker, defender);
        
        Assert.AreEqual(0, attacker.State[TemporalStatType.Blind]);
    }
}
