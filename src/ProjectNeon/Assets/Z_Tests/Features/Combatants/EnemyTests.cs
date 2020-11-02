using NUnit.Framework;
using System.Collections.Generic;

[TestFixture]
public sealed class EnemyTests
{
    [Test]
    public void CorrectEnforcerTest()
    {
        
        Enemy testEnforcer = TestableEnemy.Enforcer();
        Assert.AreEqual("Enforcer", testEnforcer.Name);
        Assert.AreEqual(2, testEnforcer.PreferredTurnOrder);
        Assert.AreEqual(1, testEnforcer.PowerLevel);
        Assert.AreEqual(50, testEnforcer.RewardCredits);
        Assert.AreEqual(BattleRole.Utility, testEnforcer.Role);
        
        // int stats tests
        Assert.AreEqual(35, testEnforcer.MaxHp);
        Assert.AreEqual(10, testEnforcer.Toughness);
        Assert.AreEqual(8, testEnforcer.Attack);
        Assert.AreEqual(0, testEnforcer.Magic);
        Assert.AreEqual(0, testEnforcer.StartingResourceAmount);
        Assert.AreEqual(1, testEnforcer.ResourceGainPerTurn);
        Assert.AreEqual(1, testEnforcer.CardsPerTurn);
        
    }

    [Test]
    public void CustomEnemyTest()
    {
        Deck customDeck = TestableObjectFactory.Create<Deck>(); 
        TurnAI customAI = TestableObjectFactory.Create<BigBadBossAI>();
        SimpleResourceType customResourceType = TestableObjectFactory.Create<SimpleResourceType>();
        
        Dictionary<string, int> intStats = new Dictionary<string, int>();
        intStats.Add("preferredTurnOrder", 1);
        intStats.Add("powerLevel", 3);
        intStats.Add("rewardCredits", 200);
        intStats.Add("maxHp", 100);
        intStats.Add("toughness", 15);
        intStats.Add("attack", 10);
        intStats.Add("magic", 0);
        intStats.Add("startingResourceAmount", 0);
        intStats.Add("resourceGainPerTurn", 2);
        intStats.Add("cardsPerTurn", 1);
        Enemy customeEnemy = TestableEnemy.
            CustomEnemy(
                customDeck,
                customAI,
                customResourceType,
                "customEnemy", 
                BattleRole.Boss, 
                true, 
                intStats, 
                5f, 
                6f
                );
        Assert.AreEqual("customEnemy", customeEnemy.Name);
        Assert.AreEqual(1, customeEnemy.PreferredTurnOrder);
        Assert.AreEqual(3, customeEnemy.PowerLevel);
        Assert.AreEqual(200, customeEnemy.RewardCredits);
        Assert.AreEqual(BattleRole.Boss, customeEnemy.Role);
        
        // int stats tests
        Assert.AreEqual(100, customeEnemy.MaxHp);
        Assert.AreEqual(15, customeEnemy.Toughness);
        Assert.AreEqual(10, customeEnemy.Attack);
        Assert.AreEqual(0, customeEnemy.Magic);
        Assert.AreEqual(0, customeEnemy.StartingResourceAmount);
        Assert.AreEqual(2, customeEnemy.ResourceGainPerTurn);
        Assert.AreEqual(1, customeEnemy.CardsPerTurn);
        
    }
    
}