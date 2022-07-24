using System;
using System.Linq;

public class PossibleEncounterChooser
{
    private readonly DeterministicRng _rng;

    //the higher this is the more effect randomness has
    private const decimal _baseEncounterValue = 200;
    //this is a cumulative bonus/penalty for every step closer to correction
    private const decimal _damageTypeRatioCorrectionAdjustment = 2;
    //this is an all or nothing bonus
    private const decimal _enemyCountBonus = 30;
    //this is the full penalty you can receive based on the percentage of enemies that were in the last fight
    private const decimal _enemyInLastFightPenalty = 90;
    //this is an all or nothing bonus
    private const decimal _newEnemyBonus = 90;
    //this is a cumulative bonus that counts the total times you have seen an enemy times by the amount of times you will
    //see them now and divide the whole result by the number of enemies in the encounter so as to not punish larger encounters
    //(keep it low)
    private const decimal _enemySeenTooOftenPenalty = 3;
    //the maximum penalty you can receive from being too easy an encounter with non linear squared function (expect 1% of the penalty to apply if its 0.9 of its strength)
    private const decimal _tooWeakPenalty = 400;
    //the maximum penalty you can recieve from being too hard an encounter with non linear squared function (expect 1% of the penalty to apply if its 1.1 of its strength)
    private const decimal _tooStrongPenalty = 600;
    //this is a cumulative bonus/penalty for every step closer to correction (comparing 3 ratios instead of 1 inflates modifications so treat it as 3 times the amount)
    private const decimal _roleRatioCorrectionAdjustment = 10;

    public PossibleEncounterChooser(DeterministicRng rng)
    {
        _rng = rng;
    }
    
    public PossibleEncounter Choose(PossibleEncounter[] encounters, PossibleEncounter[] previousEncounters, int difficulty, float flexibility, float randomness)
    {
        var possibleEncounters = encounters.Where(encounter 
            => previousEncounters.All(previous => previous.Id != encounter.Id) 
               && encounter.Power >= difficulty * (1f - flexibility)
               && encounter.Power <= difficulty * (1f + flexibility)).ToArray();
        int previousPhysicalDamageDealers = previousEncounters.Sum(x => x.PhysicalDamageDealers);
        int previousMagicDamageDealers = previousEncounters.Sum(x => x.MagicDamageDealers);
        decimal previousDamageTypeRatioScore = Math.Abs(previousPhysicalDamageDealers - previousMagicDamageDealers);
        int previousDamageDealers = previousEncounters.Sum(previousEncounter => previousEncounter.DamageDealers);
        int previousDamageMitigators = previousEncounters.Sum(previousEncounter => previousEncounter.DamageMitigators);
        int previousSpecialists = previousEncounters.Sum(previousEncounter => previousEncounter.Specialists);
        decimal previousRoleRatioScore = Math.Abs(previousDamageDealers / 4 - previousDamageMitigators) +
                                     Math.Abs(previousDamageDealers / 4 - previousSpecialists) +
                                     Math.Abs(previousDamageMitigators - previousSpecialists);
        return possibleEncounters.OrderByDescending(x => GetScore(x, previousEncounters, difficulty, previousPhysicalDamageDealers, previousMagicDamageDealers, previousDamageTypeRatioScore, previousDamageDealers, previousDamageMitigators, previousSpecialists, previousRoleRatioScore) * GetRandomModifier(randomness)).First();
    }
    
    private decimal GetScore(PossibleEncounter encounter, PossibleEncounter[] previousEncounters, int difficulty, 
            int previousPhysicalDamageDealers, int previousMagicDamageDealers, decimal previousDamageTypeRatioScore, 
            int previousDamageDealers, int previousDamageMitigators, int previousSpecialists, decimal previousRatioScore)
        => _baseEncounterValue
           + AdjustForDamageTypeRatio(encounter, previousPhysicalDamageDealers, previousMagicDamageDealers, previousDamageTypeRatioScore) 
           + BonusForDifferentNumberOfEnemiesThanLastBattle(encounter, previousEncounters)
           - PenaltyForEnemiesThatWereInLastFight(encounter, previousEncounters)
           + BonusForNeverBeforeSeenEnemy(encounter, previousEncounters)
           - PenaltyForEnemiesSeenTooOften(encounter, previousEncounters)
           - PenaltyForUnbalance(encounter, difficulty)
           + AdjustForRoleRatio(encounter, previousDamageDealers, previousDamageMitigators, previousSpecialists, previousRatioScore);

    private decimal AdjustForDamageTypeRatio(PossibleEncounter encounter, int previousPhysicalDamageDealers, int previousMagicDamageDealers, decimal previousDamageTypeRatioScore)
    {
        var ratioScore = Math.Abs((previousPhysicalDamageDealers + encounter.PhysicalDamageDealers) - (previousMagicDamageDealers + encounter.MagicDamageDealers));
        var difference = previousDamageTypeRatioScore - ratioScore;
        return difference * _damageTypeRatioCorrectionAdjustment;
    }

    private decimal BonusForDifferentNumberOfEnemiesThanLastBattle(PossibleEncounter encounter, PossibleEncounter[] previousEncounters)
        => !previousEncounters.Any() || encounter.Enemies.Length == previousEncounters[previousEncounters.Length - 1].Enemies.Length ? 0 : _enemyCountBonus;
    
    private decimal PenaltyForEnemiesThatWereInLastFight(PossibleEncounter encounter, PossibleEncounter[] previousEncounters)
        => previousEncounters.Any()
            ? encounter.Enemies.Count(enemy => previousEncounters[previousEncounters.Length - 1].Enemies.Any(previousEnemy => previousEnemy.id == enemy.id)) / (decimal)encounter.Enemies.Length * _enemyInLastFightPenalty
            : 0;

    private decimal BonusForNeverBeforeSeenEnemy(PossibleEncounter encounter, PossibleEncounter[] previousEncounters)
        => encounter.Enemies.Any(enemy => previousEncounters.All(previousEncounter =>
            previousEncounter.Enemies.All(previousEnemy => previousEnemy.id != enemy.id)))
            ? _newEnemyBonus
            : 0;

    private decimal PenaltyForEnemiesSeenTooOften(PossibleEncounter encounter, PossibleEncounter[] previousEncounters)
        => previousEncounters.Sum(previousEncounter => previousEncounter.Enemies.Sum(previousEnemy => encounter.Enemies.Count(enemy => enemy.id == previousEnemy.id))) * _enemySeenTooOftenPenalty / encounter.Enemies.Length;
    
    private decimal PenaltyForUnbalance(PossibleEncounter encounter, int difficulty)
        => encounter.Power > difficulty
            ? (decimal)Math.Pow((encounter.Power - difficulty) / (double)difficulty, 2) * _tooStrongPenalty
            : (decimal)Math.Pow(1 - (encounter.Power / (double)difficulty), 2) * _tooWeakPenalty;

    private decimal AdjustForRoleRatio(PossibleEncounter encounter, int previousDamageDealers, int previousDamageMitigators, int previousSpecialists, decimal previousRatioScore)
    {
        var damageDealers = previousDamageDealers + encounter.DamageDealers;
        var damageMitigators = previousDamageMitigators + encounter.DamageMitigators;
        var specialists = previousSpecialists + encounter.Specialists;
        var possibleRatioScore = Math.Abs(damageDealers / 4 - damageMitigators) +
                                 Math.Abs(damageDealers / 4 - specialists) +
                                 Math.Abs(damageMitigators - specialists);
        var difference = previousRatioScore - possibleRatioScore;
        return difference * _roleRatioCorrectionAdjustment;
    }

    private decimal GetRandomModifier(float randomness)
        => 1m - (decimal)_rng.Dbl() * (decimal)randomness;
}