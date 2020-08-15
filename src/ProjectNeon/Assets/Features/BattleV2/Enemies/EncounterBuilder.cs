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

    public void Init(IEnumerable<Enemy> possibleEnemies)
    {
        possible = possibleEnemies.ToArray();
    }
    
    public List<Enemy> Generate(int difficulty)
    {
        BattleLog.Write($"Started generating encounter of difficulty {difficulty}");

        var currentDifficulty = 0;
        var numRemainingMustIncludes = numMustIncludes;
        var enemies = new List<Enemy>();

        while (numRemainingMustIncludes > 0)
        {
            var nextEnemy = mustIncludePossibilities.Random();
            enemies.Add(nextEnemy);
            BattleLog.Write($"Added \"Must Include\" {nextEnemy.Name} to Encounter");
            numRemainingMustIncludes--;
            currentDifficulty = currentDifficulty + Math.Max(nextEnemy.PowerLevel, 1);
        }
        
        while (currentDifficulty < difficulty && enemies.Count < 7)
        {
            var maximum = difficulty - currentDifficulty;
            var nextEnemy = possible.Where(
                enemy => enemy.PowerLevel <= maximum
            ).Random();
            enemies.Add(nextEnemy);
            BattleLog.Write($"Added {nextEnemy.Name} to Encounter");
            currentDifficulty = currentDifficulty + Math.Max(nextEnemy.PowerLevel, 1);
        }
        
        BattleLog.Write("Finished generating encounter");
        return enemies;
    }
}
