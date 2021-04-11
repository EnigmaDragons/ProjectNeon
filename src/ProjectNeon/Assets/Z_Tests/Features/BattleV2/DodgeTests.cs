using NUnit.Framework;

[TestFixture]
public class DodgeTests
{
    private static readonly InMemoryCard BasicAttackCard = new InMemoryCard
    {
        ActionSequences = new[] { 
            CardActionSequence.Create(Scope.One, Group.Opponent,
                TestableObjectFactory.Create<CardActionsData>().Initialized(new CardActionV2(TestEffects.BasicAttack)), repeatX: false)
        }
    };
    
    [Ignore("Test Engine Doesn't Resolve Preventions Yet")]
    [Test]
    public void AttackerIsBlindAndDefenderHasDodge_DodgeStillPresent()
    {
        var defender = TestMembers.Any();
        defender.Apply(m => m.Adjust(TemporalStatType.Dodge, 1));

        var attacker = TestMembers.Any();
        attacker.Apply(m => m.Adjust(TemporalStatType.Blind, 1));

        var basicAttackCard = new Card(0, attacker, BasicAttackCard);
        basicAttackCard.Execute(new Target[] { new Single(defender),}, ResourceQuantity.None, attacker, defender);
        
        Assert.AreEqual(1, defender.State[TemporalStatType.Dodge]);
    }
    
    [Test]
    public void AttackerPlaysAttack_DodgeGoneAndHpUnchanged()
    {
        var defender = TestMembers.Any();
        defender.Apply(m => m.Adjust(TemporalStatType.Dodge, 1));

        var attacker = TestMembers.Any();

        var basicAttackCard = new Card(0, attacker, BasicAttackCard);
        basicAttackCard.Execute(new Target[] { new Single(defender),}, ResourceQuantity.None, attacker, defender);
        
        Assert.AreEqual(0, defender.State[TemporalStatType.Dodge], "Dodge Not Consumed");
        Assert.AreEqual(0, defender.State.MissingHp());
    }
    
    [Test]
    public void AttackerPlaysDoubleAttack_TwoDodgesGoneAndHpUnchanged()
    {
        var defender = TestMembers.Any();
        defender.Apply(m => m.Adjust(TemporalStatType.Dodge, 2));

        var attacker = TestMembers.Any();

        var basicAttackCard = new Card(0, attacker, new InMemoryCard
        {
            ActionSequences = new[] { 
                CardActionSequence.Create(Scope.One, Group.Opponent,
                    TestableObjectFactory.Create<CardActionsData>().Initialized(new CardActionV2(TestEffects.BasicAttack)), repeatX: false),
                CardActionSequence.Create(Scope.One, Group.Opponent,
                    TestableObjectFactory.Create<CardActionsData>().Initialized(new CardActionV2(TestEffects.BasicAttack)), repeatX: false)
            }
        });
        basicAttackCard.Execute(new Target[] { new Single(defender), new Single(defender)}, ResourceQuantity.None, attacker, defender);
        
        Assert.AreEqual(0, defender.State[TemporalStatType.Dodge], "Dodge Not Consumed");
        Assert.AreEqual(0, defender.State.MissingHp());
    }
    
    [Ignore("Test Engine Doesn't Resolve Preventions Yet")]
    [Test]
    public void AttackerIsBlindAndDefenderHasDodge_BlindGone()
    {
        var defender = TestMembers.Any();
        defender.Apply(m => m.Adjust(TemporalStatType.Dodge, 1));

        var attacker = TestMembers.Any();
        attacker.Apply(m => m.Adjust(TemporalStatType.Blind, 1));

        var basicAttackCard = new Card(0, attacker, BasicAttackCard);
        basicAttackCard.Execute(new Target[] { new Single(defender),}, ResourceQuantity.None, attacker, defender);
        
        Assert.AreEqual(0, attacker.State[TemporalStatType.Blind]);
    }
}
