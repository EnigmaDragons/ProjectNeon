using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class EncounterBuilder : ScriptableObject
{
    [SerializeField] private Enemy[] possible;
    [SerializeField] private Enemy[] mustIncludePossibilities;
    [SerializeField] private int numMustIncludes;
    [SerializeField] private int maxEnemies = 7;
    [SerializeField][Range(0, 1)] private float flexibility;
    [SerializeField][Range(0, 1)] private float minionTeamChance;

    private EncounterEnemySelector _selector => new EncounterEnemySelector(
        new StopWhenCurrentDifficultyIsEnoughRule(flexibility),
        new StopWhenMaxedOutEnemiesRule(maxEnemies),
        new OneEliteRule(),
        new MinionTeamRule(minionTeamChance, maxEnemies),
        new UniqueRolesRule(),
        new UniqueRule(),
        new LimitDifficultyRule(flexibility),
        new AddStrikerOrBruiserIfThereIsNoneRule());
    
    public void Init(IEnumerable<Enemy> possibleEnemies)
    {
        possible = possibleEnemies.ToArray();
    }
    
    public List<Enemy> Generate(int difficulty)
    {
        DevLog.Write($"Started generating encounter of difficulty {difficulty}");

        var currentDifficulty = 0;
        var numRemainingMustIncludes = numMustIncludes;
        var enemies = new List<Enemy>();

        while (numRemainingMustIncludes > 0)
        {
            var nextEnemy = mustIncludePossibilities.Random();
            enemies.Add(nextEnemy);
            DevLog.Write($"Added \"Must Include\" {nextEnemy.Name} to Encounter");
            numRemainingMustIncludes--;
            currentDifficulty = currentDifficulty + Math.Max(nextEnemy.PowerLevel, 1);
        }

        while (_selector.TryGetEnemy(
            new EncounterBuildingContext(enemies.ToArray(), possible, currentDifficulty, difficulty), out Enemy enemy))
        {
            enemies.Add(enemy);
            DevLog.Write($"Added {enemy.Name} to Encounter");
            currentDifficulty += enemy.PowerLevel;
        }

        DevLog.Write("Finished generating encounter");
        return enemies.ToList().Shuffled();
    }
}
