using System.Linq;
using NUnit.Framework;

[TestFixture]
public class PossibleEncounterBuilderTests
{
    [Test]
    public void OneEnemyWithMaxOneOnePossibleEncounter()
    {
        var enemy1 = CreateEnemy(1, BattleRole.DamageDealer, DamageType.None, EnemyTier.Normal, 1, maxCopies:1, 0);
        var possibilities = new PossibleEncounterBuilder().CalculatePossibilities(
            new[] {enemy1},
            new EnemyDissynergy[0],
            amountOfElites: 0,
            minEnemies: 1,
            maxEnemies: 1,
            maxDamageDealers: 99,
            maxDamageMitigators: 99,
            maxSpecialists: 99,
            minEncounterPower: 1,
            maxEncounterPower: 1);
        
        Assert.AreEqual(1, possibilities.Length);
        AssertPresent(possibilities, new [] { enemy1 });
    }

    [Test]
    public void OneEnemyMax99CopiesAllIterationsOfMaxEnemiesIsThere()
    {
        var enemy1 = CreateEnemy(1, BattleRole.DamageDealer, DamageType.None, EnemyTier.Normal, 1, 99, 0);
        var possibilities = new PossibleEncounterBuilder().CalculatePossibilities(
            new[] {enemy1},
            new EnemyDissynergy[0],
            amountOfElites: 0,
            minEnemies: 1,
            maxEnemies: 3,
            maxDamageDealers: 99,
            maxDamageMitigators: 99,
            maxSpecialists: 99,
            minEncounterPower: 1,
            maxEncounterPower: 99);
        
        Assert.AreEqual(3, possibilities.Length);
        AssertPresent(possibilities, new [] { enemy1 });
        AssertPresent(possibilities, new [] { enemy1, enemy1 });
        AssertPresent(possibilities, new [] { enemy1, enemy1, enemy1 });
    }

    [Test]
    public void TwoEnemiesWithMax1AllCombinationsMade()
    {
        var enemy1 = CreateEnemy(1, BattleRole.DamageDealer, DamageType.None, EnemyTier.Normal, 1, maxCopies:1, 0);
        var enemy2 = CreateEnemy(2, BattleRole.DamageDealer, DamageType.None, EnemyTier.Normal, 1, maxCopies:1, 0);
        var possibilities = new PossibleEncounterBuilder().CalculatePossibilities(
            new[] {enemy1, enemy2},
            new EnemyDissynergy[0],
            amountOfElites: 0,
            minEnemies: 1,
            maxEnemies: 3,
            maxDamageDealers: 99,
            maxDamageMitigators: 99,
            maxSpecialists: 99,
            minEncounterPower: 1,
            maxEncounterPower: 99);
        
        Assert.AreEqual(3, possibilities.Length);
        AssertPresent(possibilities, new [] { enemy1 });
        AssertPresent(possibilities, new [] { enemy2, });
        AssertPresent(possibilities, new [] { enemy1, enemy2 });
    }

    [Test]
    public void TwoEnemiesThatNeedAnAllyAndMaxOneCombinationMade()
    {
        var enemy1 = CreateEnemy(1, BattleRole.DamageDealer, DamageType.None, EnemyTier.Normal, 1, maxCopies:1, minAllies:1);
        var enemy2 = CreateEnemy(2, BattleRole.DamageDealer, DamageType.None, EnemyTier.Normal, 1, maxCopies:1, minAllies:1);
        var possibilities = new PossibleEncounterBuilder().CalculatePossibilities(
            new[] {enemy1, enemy2},
            new EnemyDissynergy[0],
            amountOfElites: 0,
            minEnemies: 1,
            maxEnemies: 3,
            maxDamageDealers: 99,
            maxDamageMitigators: 99,
            maxSpecialists: 99,
            minEncounterPower: 1,
            maxEncounterPower: 99);
        
        Assert.AreEqual(1, possibilities.Length);
        AssertPresent(possibilities, new [] { enemy1, enemy2 });
    }

    [Test]
    public void TwoEnemiesDissynergizeThoseCombinationsNotMade()
    {
        var enemy1 = CreateEnemy(1, BattleRole.DamageDealer, DamageType.None, EnemyTier.Normal, 1, maxCopies:1, 0);
        var enemy2 = CreateEnemy(2, BattleRole.DamageDealer, DamageType.None, EnemyTier.Normal, 1, maxCopies:1, 0);
        var possibilities = new PossibleEncounterBuilder().CalculatePossibilities(
            new[] {enemy1, enemy2},
            new [] { new EnemyDissynergy { enemy1 = enemy1, enemy2 = enemy2 }},
            amountOfElites: 0,
            minEnemies: 1,
            maxEnemies: 3,
            maxDamageDealers: 99,
            maxDamageMitigators: 99,
            maxSpecialists: 99,
            minEncounterPower: 1,
            maxEncounterPower: 99);
        
        Assert.AreEqual(2, possibilities.Length);
        AssertPresent(possibilities, new [] { enemy1 });
        AssertPresent(possibilities, new [] { enemy2, });
    }
    
    [TestCase(1, 1, 1, 4)]
    [TestCase(1, 1, 2, 5)]
    [TestCase(1, 2, 1, 5)]
    [TestCase(1, 2, 2, 6)]
    [TestCase(2, 1, 1, 7)]
    [TestCase(2, 1, 2, 8)]
    [TestCase(2, 2, 1, 8)]
    [TestCase(2, 2, 2, 9)]
    public void MaximumEnemyTypesEnsurePartyRestrictions(int maxDamageDealers, int maxDamageMitigators, int maxSpecialists, int expectedCombinations)
    {
        var enemy1 = CreateEnemy(1, BattleRole.DamageDealer, DamageType.None, EnemyTier.Normal, 1, 99, 0);
        var enemy2 = CreateEnemy(2, BattleRole.Survivability, DamageType.None, EnemyTier.Normal, 1, 99, 0);
        var enemy3 = CreateEnemy(3, BattleRole.Specialist, DamageType.None, EnemyTier.Normal, 1, 99, 0);
        var possibilities = new PossibleEncounterBuilder().CalculatePossibilities(
            new[] {enemy1, enemy2, enemy3},
            new EnemyDissynergy[0],
            amountOfElites: 0,
            minEnemies: 1,
            maxEnemies: 3,
            maxDamageDealers: maxDamageDealers,
            maxDamageMitigators: maxDamageMitigators,
            maxSpecialists: maxSpecialists,
            minEncounterPower: 1,
            maxEncounterPower: 99);
        
        Assert.AreEqual(expectedCombinations, possibilities.Length);
        if (maxDamageDealers > 0)
            AssertPresent(possibilities, new [] { enemy1 });
        if (maxDamageDealers > 1)
            AssertPresent(possibilities, new [] { enemy1, enemy1 });
        if (maxDamageDealers > 0 && maxDamageMitigators > 0)
            AssertPresent(possibilities, new [] { enemy1, enemy2 });
        if (maxDamageDealers > 0 && maxSpecialists > 0)
            AssertPresent(possibilities, new [] { enemy1, enemy3 });
        if (maxDamageDealers > 2)
            AssertPresent(possibilities, new [] { enemy1, enemy1, enemy1 });
        if (maxDamageDealers > 1 && maxDamageMitigators > 0)
            AssertPresent(possibilities, new [] { enemy1, enemy1, enemy2 });
        if (maxDamageDealers > 1 && maxSpecialists > 0)
            AssertPresent(possibilities, new [] { enemy1, enemy1, enemy3 });
        if (maxDamageDealers > 0 && maxDamageMitigators > 1)
            AssertPresent(possibilities, new [] { enemy1, enemy2, enemy2 });
        if (maxDamageDealers > 0 && maxDamageMitigators > 0 && maxSpecialists > 0)
            AssertPresent(possibilities, new [] { enemy1, enemy2, enemy3 });
        if (maxDamageDealers > 0 && maxSpecialists > 1)
            AssertPresent(possibilities, new [] { enemy1, enemy3, enemy3 });
    }

    [Test]
    public void MinAndMaxEncounterPowerHonored()
    {
        var enemy1 = CreateEnemy(1, BattleRole.DamageDealer, DamageType.None, EnemyTier.Normal, powerLevel: 2, 99, 0);
        var possibilities = new PossibleEncounterBuilder().CalculatePossibilities(
            new[] {enemy1},
            new EnemyDissynergy[0],
            amountOfElites: 0,
            minEnemies: 1,
            maxEnemies: 4,
            maxDamageDealers: 99,
            maxDamageMitigators: 99,
            maxSpecialists: 99,
            minEncounterPower: 3,
            maxEncounterPower: 7);
        
        Assert.AreEqual(2, possibilities.Length);
        AssertPresent(possibilities, new [] { enemy1, enemy1 });
        AssertPresent(possibilities, new [] { enemy1, enemy1, enemy1 });
    }

    [Test]
    public void RequireAnEliteEliteAlwaysPresent()
    {
        var enemy1 = CreateEnemy(1, BattleRole.DamageDealer, DamageType.None, EnemyTier.Elite, 1, 99, 0);
        var enemy2 = CreateEnemy(2, BattleRole.DamageDealer, DamageType.None, EnemyTier.Normal, 1, 99, 0);
        var possibilities = new PossibleEncounterBuilder().CalculatePossibilities(
            new[] {enemy1, enemy2},
            new EnemyDissynergy[0],
            amountOfElites: 1,
            minEnemies: 1,
            maxEnemies: 3,
            maxDamageDealers: 99,
            maxDamageMitigators: 99,
            maxSpecialists: 99,
            minEncounterPower: 1,
            maxEncounterPower: 99);
        
        Assert.AreEqual(3, possibilities.Length);
        AssertPresent(possibilities, new [] { enemy1 });
        AssertPresent(possibilities, new [] { enemy1, enemy2 });
        AssertPresent(possibilities, new [] { enemy1, enemy2, enemy2 });
    }

    [Test]
    public void AlwaysHasOneDamageDealer()
    {
        var enemy1 = CreateEnemy(1, BattleRole.DamageDealer, DamageType.None, EnemyTier.Normal, 1, 99, 0);
        var enemy2 = CreateEnemy(2, BattleRole.Specialist, DamageType.None, EnemyTier.Normal, 1, 99, 0);
        var possibilities = new PossibleEncounterBuilder().CalculatePossibilities(
            new[] {enemy1, enemy2},
            new EnemyDissynergy[0],
            amountOfElites: 0,
            minEnemies: 1,
            maxEnemies: 2,
            maxDamageDealers: 99,
            maxDamageMitigators: 99,
            maxSpecialists: 99,
            minEncounterPower: 1,
            maxEncounterPower: 99);
        
        Assert.AreEqual(3, possibilities.Length);
        AssertPresent(possibilities, new [] { enemy1 });
        AssertPresent(possibilities, new [] { enemy1, enemy1 });
        AssertPresent(possibilities, new [] { enemy1, enemy2 });
    }

    [Test]
    public void EliteIsAlwayTheHighestPowerOnTeam()
    {
        var enemy1 = CreateEnemy(1, BattleRole.DamageDealer, DamageType.None, EnemyTier.Elite, 2, 99, 0);
        var enemy2 = CreateEnemy(2, BattleRole.DamageDealer, DamageType.None, EnemyTier.Normal, 1, 99, 0);
        var enemy3 = CreateEnemy(3, BattleRole.DamageDealer, DamageType.None, EnemyTier.Normal, 3, 99, 0);
        var possibilities = new PossibleEncounterBuilder().CalculatePossibilities(
            new[] {enemy1, enemy2, enemy3},
            new EnemyDissynergy[0],
            amountOfElites: 1,
            minEnemies: 1,
            maxEnemies: 2,
            maxDamageDealers: 99,
            maxDamageMitigators: 99,
            maxSpecialists: 99,
            minEncounterPower: 1,
            maxEncounterPower: 99);
        
        Assert.AreEqual(2, possibilities.Length);
        AssertPresent(possibilities, new [] { enemy1 });
        AssertPresent(possibilities, new [] { enemy1, enemy2 });
    }

    private Enemy CreateEnemy(int id, BattleRole role, DamageType damageType, EnemyTier tier, int powerLevel, int maxCopies, int minAllies)
        => TestableObjectFactory.Create<Enemy>().InitializedForTest(id, role, damageType, tier, powerLevel, maxCopies, minAllies);

    private void AssertPresent(PossibleEncounter[] possibilities, Enemy[] enemiesPresent)
        => Assert.True(possibilities.Any(x 
            => x.Enemies.Length == enemiesPresent.Length 
            && enemiesPresent.GroupBy(enemy => enemy.id).All(enemyGroup => x.Enemies.Count(enemy => enemy.id == enemyGroup.Key) == enemyGroup.Count())
            && x.Power == enemiesPresent.Sum(enemy => enemy.stageDetails[0].powerLevel) 
            && x.PhysicalDamageDealers == enemiesPresent.Count(enemy => enemy.DamageType == DamageType.Physical)
            && x.MagicDamageDealers == enemiesPresent.Count(enemy => enemy.DamageType == DamageType.Magic)));
}