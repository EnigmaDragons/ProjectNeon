using System.Collections.Generic;
using System.Linq;

public class PossibleEncounterBuilder
{
    public PossibleEncounter[] CalculatePossibilities(Enemy[] enemies,
        EnemyDissynergy[] dissynergies,
        int amountOfElites,
        int minEnemies,
        int maxEnemies,
        int maxDamageDealers,
        int maxDamageMitigators,
        int maxSpecialists,
        int minEncounterPower,
        int maxEncounterPower)
    {
        var possibilities = new List<PossibleEncounter>();
        for (int enemiesAmount = minEnemies; enemiesAmount <= maxEnemies; enemiesAmount++)
            possibilities.AddRange(AddAnotherEnemy(enemiesAmount, enemies, dissynergies, amountOfElites, maxDamageDealers, maxDamageMitigators, maxSpecialists, minEncounterPower, maxEncounterPower, new Enemy[0], 0));
        return possibilities.OrderBy(x => x.Power).ToArray();
    }

    private List<PossibleEncounter> AddAnotherEnemy(
            int enemyAmount,
            Enemy[] enemies,
            EnemyDissynergy[] dissynergies,
            int amountOfElites,
            int maxDamageDealers,
            int maxDamageMitigators,
            int maxSpecialists,
            int minEncounterPower,
            int maxEncounterPower,
            Enemy[] currentlySelectedEnemies,
            int enemyIndex)
    {
        var possibilities = new List<PossibleEncounter>();
        var power = currentlySelectedEnemies.Sum(x => x.stageDetails[0].powerLevel);
        for (; enemyIndex < enemies.Length; enemyIndex++)
        {
            var enemy = enemies[enemyIndex];
            if (IsAnEliteThatCannotFit(enemy, currentlySelectedEnemies, amountOfElites)
                || IsDissynergizedByAnotherSelectedEnemy(enemy, currentlySelectedEnemies, dissynergies)
                || IsBattleRoleAndBattleRoleCannotFit(enemy, currentlySelectedEnemies, maxDamageDealers, maxDamageMitigators, maxSpecialists)
                || WouldExceedMaxPower(enemy, power, maxEncounterPower)
                || IsLastEnemyToBeAddedAndWouldBeLessThanMinimumPower(enemy, currentlySelectedEnemies, power, minEncounterPower, enemyAmount)
                || AlreadyHasMaxCopiesOfEnemy(enemy, currentlySelectedEnemies)
                || CannotHaveEnoughAlliesForThisEnemy(enemy, enemyAmount)
                || IsLastEnemyToBeAddedAndWouldNotHaveEnoughElites(enemy, currentlySelectedEnemies, enemyAmount, amountOfElites)
                || IsLastEnemyToBeAddedAndThereIsNoDamageDealer(enemy, currentlySelectedEnemies, enemyAmount)
                || IsNotEliteAndStrongerThanElitesPresent(enemy, currentlySelectedEnemies)
                || IsEliteAndNotStrongerThanNonElitesPresent(enemy, currentlySelectedEnemies))
                continue;
            var newSelection = currentlySelectedEnemies.Concat(enemy).ToArray();
            if (IsLastEnemyToBeAdded(currentlySelectedEnemies, enemyAmount))
                possibilities.Add(CreatePossibility(newSelection));
            else
                possibilities.AddRange(AddAnotherEnemy(enemyAmount, enemies, dissynergies, amountOfElites, maxDamageDealers, maxDamageMitigators, maxSpecialists, minEncounterPower, maxEncounterPower, newSelection, enemyIndex));
        }
        return possibilities;
    }
    
    private bool IsAnEliteThatCannotFit(Enemy enemy, Enemy[] currentlySelectedEnemies, int amountOfElites)
        => enemy.Tier == EnemyTier.Elite && currentlySelectedEnemies.Count(x => x.Tier == EnemyTier.Elite) == amountOfElites;
    
    private bool IsDissynergizedByAnotherSelectedEnemy(Enemy enemy, Enemy[] currentlySelectedEnemies, EnemyDissynergy[] dissynergies)
        => currentlySelectedEnemies.Any() 
        && dissynergies.Any(dissynergy 
            => (dissynergy.enemy1 == enemy && currentlySelectedEnemies.Any(selectedEnemy => selectedEnemy == dissynergy.enemy2))
            || (dissynergy.enemy2 == enemy && currentlySelectedEnemies.Any(selectedEnemy => selectedEnemy == dissynergy.enemy1)));

    private bool IsBattleRoleAndBattleRoleCannotFit(Enemy enemy, Enemy[] currentlySelectedEnemies, int maxDamageDealers, int maxDamageMitigators, int maxSpecialists)
        => IsBattleRoleAndBattleRoleCannotFit(enemy, currentlySelectedEnemies, BattleRole.DamageDealer, maxDamageDealers)
        || IsBattleRoleAndBattleRoleCannotFit(enemy, currentlySelectedEnemies, BattleRole.Survivability, maxDamageMitigators)
        || IsBattleRoleAndBattleRoleCannotFit(enemy, currentlySelectedEnemies, BattleRole.Specialist, maxSpecialists);
    
    private bool IsBattleRoleAndBattleRoleCannotFit(Enemy enemy, Enemy[] currentlySelectedEnemies, BattleRole role, int maxOfRole)
        => enemy.BattleRole == role && currentlySelectedEnemies.Count(x => x.BattleRole == role) == maxOfRole;

    private bool WouldExceedMaxPower(Enemy enemy, int currentPower, int maxPower)
        => currentPower + enemy.stageDetails[0].powerLevel > maxPower;
    
    private bool IsLastEnemyToBeAddedAndWouldBeLessThanMinimumPower(Enemy enemy, Enemy[] currentlySelectedEnemies, int currentPower, int minPower, int enemyAmount)
        => IsLastEnemyToBeAdded(currentlySelectedEnemies, enemyAmount) && currentPower + enemy.stageDetails[0].powerLevel < minPower;

    private bool AlreadyHasMaxCopiesOfEnemy(Enemy enemy, Enemy[] currentlySelectedEnemies)
        => currentlySelectedEnemies.Count(x => x.id == enemy.id) == enemy.MaxCopies;

    private bool CannotHaveEnoughAlliesForThisEnemy(Enemy enemy, int enemyAmount)
        => enemy.MinimumAllies >= enemyAmount;
    
    private bool IsLastEnemyToBeAddedAndWouldNotHaveEnoughElites(Enemy enemy, Enemy[] currentlySelectedEnemies, int enemyAmount, int amountOfElites)
        => IsLastEnemyToBeAdded(currentlySelectedEnemies, enemyAmount) && currentlySelectedEnemies.Concat(enemy).Count(x => x.Tier == EnemyTier.Elite) != amountOfElites;
    
    private bool IsLastEnemyToBeAddedAndThereIsNoDamageDealer(Enemy enemy, Enemy[] currentlySelectedEnemies, int enemyAmount)
        => IsLastEnemyToBeAdded(currentlySelectedEnemies, enemyAmount) && enemy.BattleRole != BattleRole.DamageDealer && currentlySelectedEnemies.All(x => x.BattleRole != BattleRole.DamageDealer);
    
    private bool IsNotEliteAndStrongerThanElitesPresent(Enemy enemy, Enemy[] currentlySelectedEnemies)
        => enemy.Tier != EnemyTier.Elite && currentlySelectedEnemies.Any(x => x.Tier == EnemyTier.Elite && x.stageDetails[0].powerLevel < enemy.stageDetails[0].powerLevel);
    
    private bool IsEliteAndNotStrongerThanNonElitesPresent(Enemy enemy, Enemy[] currentlySelectedEnemies)
        => enemy.Tier == EnemyTier.Elite && currentlySelectedEnemies.Any(x => x.Tier != EnemyTier.Elite && x.stageDetails[0].powerLevel > enemy.stageDetails[0].powerLevel);

    private bool IsLastEnemyToBeAdded(Enemy[] currentlySelectedEnemies, int enemyAmount)
        => currentlySelectedEnemies.Length + 1 == enemyAmount;

    private PossibleEncounter CreatePossibility(Enemy[] enemies)
    {
        var orderedEnemies = enemies.OrderBy(x => x.id).ToArray();
        return new PossibleEncounter
        {
            Id = string.Join("", orderedEnemies.Select(x => $"{x.id:00000}")),
            Enemies = orderedEnemies,
            Power = orderedEnemies.Sum(x => x.stageDetails[0].powerLevel),
            PhysicalDamageDealers = orderedEnemies.Count(x => x.DamageType == DamageType.Physical),
            MagicDamageDealers = orderedEnemies.Count(x => x.DamageType == DamageType.Magic),
            DamageDealers = orderedEnemies.Count(x => x.BattleRole == BattleRole.DamageDealer),
            DamageMitigators = orderedEnemies.Count(x => x.BattleRole == BattleRole.Survivability),
            Specialists = orderedEnemies.Count(x => x.BattleRole == BattleRole.Specialist),
        };

    }
}