using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class EncounterBuilder : ScriptableObject
{
    [SerializeField] private bool allowElites = true;
    [SerializeField] private Enemy[] possible;
    [SerializeField] private Enemy[] mustIncludePossibilities;
    [SerializeField] private int numMustIncludes;
    [SerializeField] private int maxEnemies = 7;
    [SerializeField][Range(0, 1)] private float flexibility;
    [SerializeField][Range(0, 1)] private float minionTeamChance;
    [SerializeField] private AdventureProgress2 currentAdventureProgress;

    private bool _debugLog = false;

    private EncounterEnemySelector _selector => new EncounterEnemySelector(
        new NoPowerLevelZeroEnemiesRule(),
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
        DebugLog($"Started generating encounter of difficulty {difficulty}");

        var currentDifficulty = 0;
        var numRemainingMustIncludes = numMustIncludes;
        var enemies = new List<EnemyInstance>();

        while (numRemainingMustIncludes > 0)
        {
            var nextEnemy = mustIncludePossibilities.Random();
            var nextEnemyInstance = nextEnemy.ForStage(currentAdventureProgress.CurrentChapterNumber);
            enemies.Add(nextEnemyInstance);
            DebugLog($"Added \"Must Include\" {nextEnemyInstance.Name} to Encounter");
            numRemainingMustIncludes--;
            currentDifficulty = currentDifficulty + Math.Max(nextEnemyInstance.PowerLevel, 1);
        }

        if (!allowElites && possible.Any(p => p.Tier == EnemyTier.Elite))
            Log.Error($"{name} has Elite Enemies but isn't an Elite pool.");
        var possibleEnemies = possible
            .Where(x => allowElites || x.Tier != EnemyTier.Elite)
            .Select(x => x.ForStage(currentAdventureProgress.CurrentChapterNumber))
            .ToArray();
        while (_selector.TryGetEnemy(new EncounterBuildingContext(enemies.ToArray(), possibleEnemies, currentDifficulty, difficulty), out EnemyInstance enemy))
        {
            enemies.Add(enemy);
            DebugLog($"Added {enemy.Name} to Encounter");
            currentDifficulty += enemy.PowerLevel;
        }

        DebugLog("Finished generating encounter");
        return enemies.Select(x => x).ToList().Shuffled();
    }

    private void DebugLog(string msg)
    {
        if (_debugLog)
            DevLog.Write(msg);
    }
}
