using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

[TestFixture]
public class PossibleEncounterChooserTests
{
    [Test]
    public void GivesOnlyOneEncounterWithinFlexibilityRange()
    {
        var encounter1 = CreatePossibility("1", 100, CreateEnemy(1));
        var encounter2 = CreatePossibility("2", 0, CreateEnemy(2));

        AssertAlwaysChooses(encounter1, new [] {encounter1, encounter2}, new PossibleEncounter[0], 90, 0.3f, 1);
    }

    [Test]
    public void EncounterWithEnemyYouHaveFacedBeforeNotChosen()
    {
        var enemy1 = CreateEnemy(1);
        var enemy2 = CreateEnemy(2);
        var encounter1 = CreatePossibility("1", 100, enemy1);
        var encounter2 = CreatePossibility("2", 100, enemy1, enemy1);
        var encounter3 = CreatePossibility("3", 100, enemy2, enemy2);
        
        AssertAlwaysChooses(encounter3, new [] {encounter2, encounter3}, new [] {encounter1}, 100, 0.1f, 0);
    }

    [Test]
    public void EncounterWithMagicDamageDealerWhenYouFacedMorePhysicalPreviouslyAlwaysChosen()
    {
        var physicalEnemy1 = CreateEnemy(1, DamageType.Physical);
        var physicalEnemy2 = CreateEnemy(2, DamageType.Physical);
        var magicEnemy1 = CreateEnemy(3, DamageType.Magic);
        var magicEnemy2 = CreateEnemy(4, DamageType.Magic);
        var encounter1 = CreatePossibility("1", 100, physicalEnemy1, physicalEnemy1);
        var encounter2 = CreatePossibility("2", 100, magicEnemy1);
        var encounter3 = CreatePossibility("3", 100, physicalEnemy2);
        var encounter4 = CreatePossibility("4", 100, magicEnemy2);
        
        AssertAlwaysChooses(encounter4, new [] {encounter3, encounter4}, new [] {encounter1, encounter2}, 100, 0.1f, 0);
    }

    [Test]
    public void EncounterWithMoreEnemiesThanLastTimeAlwaysChosen()
    {
        var enemy1 = CreateEnemy(1);
        var enemy2 = CreateEnemy(2);
        var enemy3 = CreateEnemy(3);
        var encounter1 = CreatePossibility("1", 100, enemy1);
        var encounter2 = CreatePossibility("2", 100, enemy2);
        var encounter3 = CreatePossibility("3", 100, enemy3, enemy3);
        
        AssertAlwaysChooses(encounter3, new [] {encounter2, encounter3}, new [] {encounter1}, 100, 0.1f, 0);
    }

    [Test]
    public void EncounterWithCloserToTargetBalanceAlwaysChosen()
    {
        var enemy1 = CreateEnemy(1);
        var enemy2 = CreateEnemy(2);
        var encounter1 = CreatePossibility("1", 100, enemy1);
        var encounter2 = CreatePossibility("2", 80, enemy2);
        
        AssertAlwaysChooses(encounter1, new [] {encounter1, encounter2}, new PossibleEncounter[0], 95, 0.3f, 0);
    }

    [Test]
    public void EncounterThatYouHaveFacedBeforeNeverChosen()
    {
        var enemy1 = CreateEnemy(1);
        var encounter1 = CreatePossibility("1", 100, enemy1);
        var encounter2 = CreatePossibility("2", 100, enemy1);
        
        AssertAlwaysChooses(encounter2, new [] {encounter1, encounter2}, new [] { encounter1 }, 100, 0.1f, 1);
    }

    [Test]
    public void EncounterWithEnemyYouJustFacedNotChosenOverMoreSeenEnemy()
    {
        var enemy1 = CreateEnemy(1);
        var enemy2 = CreateEnemy(2);
        var encounter1 = CreatePossibility("1", 100, enemy1, enemy1);
        var encounter2 = CreatePossibility("2", 100, enemy2);
        var encounter3 = CreatePossibility("3", 100, enemy1, enemy1, enemy1);
        var encounter4 = CreatePossibility("4", 100, enemy2, enemy2, enemy2);
        
        AssertAlwaysChooses(encounter3, new [] {encounter3, encounter4}, new [] { encounter1, encounter2 }, 100, 0.1f, 0);
    }

    [Test]
    public void EncounterWithNewEnemyChosenOverCaringAboutPhysicalOrMagicSeen()
    {
        var physicalEnemy1 = CreateEnemy(1, DamageType.Physical);
        var physicalEnemy2 = CreateEnemy(2, DamageType.Physical);
        var magicEnemy = CreateEnemy(3, DamageType.Magic);
        var encounter1 = CreatePossibility("1", 100, physicalEnemy1, physicalEnemy1, magicEnemy);
        var encounter2 = CreatePossibility("2", 100, physicalEnemy2);
        var encounter3 = CreatePossibility("3", 100, magicEnemy);
        
        AssertAlwaysChooses(encounter2, new [] {encounter2, encounter3}, new [] { encounter1 }, 100, 0.1f, 0);
    }

    [Test]
    public void DifferentNumberChosenOverPhysicalOrMagicSeen()
    {
        var physicalEnemy = CreateEnemy(1, DamageType.Physical);
        var magicEnemy = CreateEnemy(3, DamageType.Magic);
        var encounter1 = CreatePossibility("1", 100, physicalEnemy, physicalEnemy, magicEnemy);
        var encounter2 = CreatePossibility("2", 100, magicEnemy, magicEnemy, magicEnemy);
        var encounter3 = CreatePossibility("3", 100, physicalEnemy);
        
        AssertAlwaysChooses(encounter3, new [] {encounter2, encounter3}, new [] { encounter1 }, 100, 0.1f, 0);
    }

    [Test]
    public void NewEnemyChosenOverDifferentEnemyCountNumber()
    {
        var enemy1 = CreateEnemy(1);
        var enemy2 = CreateEnemy(2);
        var encounter1 = CreatePossibility("1", 100, enemy1);
        var encounter2 = CreatePossibility("2", 100, enemy1, enemy1);
        var encounter3 = CreatePossibility("3", 100, enemy2);
        
        AssertAlwaysChooses(encounter3, new [] {encounter2, encounter3}, new [] { encounter1 }, 100, 0.1f, 0);
    }

    [Test]
    public void NewEnemyWithMoreSeenEnemyChosenOverOnceSeenEnemy()
    {
        var enemy1 = CreateEnemy(1);
        var enemy2 = CreateEnemy(2);
        var enemy3 = CreateEnemy(3);
        var enemy4 = CreateEnemy(4);
        var encounter1 = CreatePossibility("1", 100, enemy1, enemy1, enemy1, enemy2);
        var encounter2 = CreatePossibility("2", 100, enemy3);
        var encounter3 = CreatePossibility("3", 100, enemy4, enemy1);
        var encounter4 = CreatePossibility("4", 100, enemy2, enemy2);
        
        AssertAlwaysChooses(encounter3, new [] {encounter3, encounter4}, new [] { encounter1, encounter2 }, 100, 0.1f, 0);
    }

    [Test]
    public void WhenTheRatioOfRolesIsOffItWantsToCorrectIt()
    {
        var damageDealer1 = CreateEnemy(1, DamageType.Other, BattleRole.DamageDealer);
        var damageDealer2 = CreateEnemy(2, DamageType.Other, BattleRole.DamageDealer);
        var damageMitigator1 = CreateEnemy(3, DamageType.None, BattleRole.Survivability);
        var damageMitigator2 = CreateEnemy(4, DamageType.None, BattleRole.Survivability);
        var specialist1 = CreateEnemy(5, DamageType.None, BattleRole.Specialist);
        var specialist2 = CreateEnemy(6, DamageType.None, BattleRole.Specialist);
        var encounter1 = CreatePossibility("1", 100, damageDealer1, damageDealer1, specialist1, damageMitigator1);
        var encounter2 = CreatePossibility("2", 100, damageDealer1, damageDealer1, damageDealer1, damageMitigator1);
        var encounter3 = CreatePossibility("3", 100, damageDealer1, damageDealer1, damageDealer1, damageDealer1);
        var encounter4 = CreatePossibility("4", 100, damageDealer2, damageDealer2);
        var encounter5 = CreatePossibility("5", 100, damageDealer2, damageMitigator2);
        var encounter6 = CreatePossibility("6", 100, damageDealer2, specialist2);
        
        AssertAlwaysChooses(encounter6, new [] {encounter4, encounter5, encounter6}, new [] { encounter1, encounter2, encounter3 }, 100, 0.1f, 0);
    }

    [Test]
    public void RandomnessForcesUnpredictable()
    {
        var enemy1 = CreateEnemy(1, DamageType.Physical, BattleRole.DamageDealer);
        var enemy2 = CreateEnemy(2, DamageType.Magic, BattleRole.DamageDealer);
        var enemy3 = CreateEnemy(3, DamageType.Physical, BattleRole.DamageDealer);
        var enemy4 = CreateEnemy(4, DamageType.Physical, BattleRole.Specialist);
        var firstEncounter = CreatePossibility("1", 100, enemy1, enemy1);
        var secondEncounter = CreatePossibility("2", 100, enemy2);
        var unbalancedEncounter = CreatePossibility("3", 50, enemy1);
        var diffDamageTypeEncounter = CreatePossibility("4", 100, enemy2);
        var diffEnemyCountEncounter = CreatePossibility("5", 100, enemy1, enemy1);
        var newEnemyEncounter = CreatePossibility("6", 100, enemy3);
        var specialistEncounter = CreatePossibility("7", 100, enemy1, enemy4);
        
        AssertChoosesAtSomePoint(new [] { firstEncounter, secondEncounter, unbalancedEncounter, diffDamageTypeEncounter, diffEnemyCountEncounter, newEnemyEncounter, specialistEncounter }, new [] { firstEncounter, secondEncounter }, 100, 0.6f, 1, 
            unbalancedEncounter, diffDamageTypeEncounter, diffEnemyCountEncounter, newEnemyEncounter, specialistEncounter);
    }

    private PossibleEncounter CreatePossibility(string id, int powerLevel, params Enemy[] enemies)
        => new PossibleEncounter
        {
            Id = id, 
            Power = powerLevel, 
            Enemies = enemies, 
            PhysicalDamageDealers = enemies.Count(x => x.DamageType == DamageType.Physical),
            MagicDamageDealers = enemies.Count(x => x.DamageType == DamageType.Magic),
            DamageDealers = enemies.Count(x => x.BattleRole == BattleRole.DamageDealer),
            DamageMitigators = enemies.Count(x => x.BattleRole == BattleRole.Survivability),
            Specialists = enemies.Count(x => x.BattleRole == BattleRole.Specialist),
        };
    
    private Enemy CreateEnemy(int id, DamageType damageType = DamageType.Other, BattleRole role = BattleRole.DamageDealer)
        => TestableObjectFactory.Create<Enemy>().InitializedForTest(id, role, damageType, EnemyTier.Normal, 1, 99, 0);
    
    private void AssertAlwaysChooses(PossibleEncounter expected, PossibleEncounter[] encounters, PossibleEncounter[] previousEncounters, int difficulty, float flexibility, float randomness)
    {
        for (var i = 0; i < 10; i++)
        {
            var choice = new PossibleEncounterChooser().Choose(encounters, previousEncounters, difficulty, flexibility, randomness);
            Assert.AreEqual(expected.Id, choice.Id);
        }
    }

    private void AssertChoosesAtSomePoint(PossibleEncounter[] encounters, PossibleEncounter[] previousEncounters, int difficulty, float flexibility, float randomness, params PossibleEncounter[] encountersToEventuallyChoose)
    {
        var encountersNotChosen = encountersToEventuallyChoose.ToList();
        var encountersChosen = new List<PossibleEncounter>();
        for (var i = 0; i < 100000; i++)
        {
            var choice = new PossibleEncounterChooser().Choose(encounters, previousEncounters, difficulty, flexibility, randomness);
            if (encountersNotChosen.Contains(choice))
            {
                encountersNotChosen.Remove(choice);
                encountersChosen.Add(choice);
                if (!encountersNotChosen.Any())
                {
                    Assert.True(true);
                    return;
                }
            }
            else if (!encountersChosen.Contains(choice))
            {
                Assert.True(false, $"An unexpected encounter was chosen: {choice.Id}");
                return;
            }
        }
        Assert.True(false, $"Some Encounters Were Expected To Be Chosen But Were Not: {string.Join(" ", encountersNotChosen.Select(x => x.Id.ToString()))}");
    }
}