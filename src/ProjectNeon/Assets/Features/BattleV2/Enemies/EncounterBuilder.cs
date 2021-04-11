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
    [SerializeField] private AdventureProgress2 currentAdventureProgress;

    private bool _debugLog = false;

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
    
    public List<EnemyInstance> Generate(int difficulty)
    {
        Log($"Started generating encounter of difficulty {difficulty}");

        var currentDifficulty = 0;
        var numRemainingMustIncludes = numMustIncludes;
        var enemies = new List<EnemyInstance>();

        while (numRemainingMustIncludes > 0)
        {
            var nextEnemy = mustIncludePossibilities.Random();
            var nextEnemyInstance = nextEnemy.GetEnemy(currentAdventureProgress.Stage);
            enemies.Add(nextEnemyInstance);
            Log($"Added \"Must Include\" {nextEnemyInstance.Name} to Encounter");
            numRemainingMustIncludes--;
            currentDifficulty = currentDifficulty + Math.Max(nextEnemyInstance.PowerLevel, 1);
        }

        while (_selector.TryGetEnemy(
            new EncounterBuildingContext(enemies.ToArray(), possible.Select(x => x.GetEnemy(currentAdventureProgress.Stage)).ToArray(), currentDifficulty, difficulty), out EnemyInstance enemy))
        {
            enemies.Add(enemy);
            Log($"Added {enemy.Name} to Encounter");
            currentDifficulty += enemy.PowerLevel;
        }

        Log("Finished generating encounter");
        return enemies.Select(x => x).ToList().Shuffled();
    }

    private void Log(string msg)
    {
        if (_debugLog)
            DevLog.Write(msg);
    }
}
