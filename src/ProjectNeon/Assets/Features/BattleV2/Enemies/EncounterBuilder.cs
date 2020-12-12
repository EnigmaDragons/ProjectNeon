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

        var min = difficulty * (1 - flexibility);
        var max = difficulty * (1 + flexibility);
        
        while (currentDifficulty < min && enemies.Count < maxEnemies)
        {
            var maximum = max - currentDifficulty;
            var enemyRolesOverrepresented = enemies
                .GroupBy(e => e.Role)
                .Where(g => g.Sum(e => e.PowerLevel) >= (difficulty / 2f))
                .Select(e => e.Key);
            var uniqueEnemies = enemies.Where(e => e.IsUnique);
            var filteredOptions = possible
                .Where(e => !enemyRolesOverrepresented.Contains(e.Role))
                .Where(e => !uniqueEnemies.Contains(e))
                .Where(e => e.PowerLevel <= maximum);

            var nextEnemy = filteredOptions.Random();
            enemies.Add(nextEnemy);
            BattleLog.Write($"Added {nextEnemy.Name} to Encounter");
            currentDifficulty += nextEnemy.PowerLevel;
        }
        
        BattleLog.Write("Finished generating encounter");
        return enemies.ToList().Shuffled();
    }
}
