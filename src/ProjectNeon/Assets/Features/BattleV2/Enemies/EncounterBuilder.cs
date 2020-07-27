using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * Generates an anemy encounter based on the difficulty set.
 * 
 * The script will add a random enemy up to the difficult level set and
 * will add more enemies until the desired difficulty is reached, or the
 * number of enemies is 7. it will also allow duplicate enemy types.
 */

[CreateAssetMenu]
public class EncounterBuilder : ScriptableObject
{
    [SerializeField] private IntReference difficulty;
    [SerializeField] private Enemy[] possible;

    public void Init(IEnumerable<Enemy> possibleEnemies, int newDifficulty)
    {
        possible = possibleEnemies.ToArray();
        difficulty = new IntReference(newDifficulty);
    }
    
    public List<Enemy> Generate()
    {
        BattleLog.Write("Generated Encounter");
        /**
         * @todo #52:30min Evolve Encounter Generation after playtesting. 
         */

        var currentDifficulty = 0;
        var enemies = new List<Enemy>();

        while (currentDifficulty < difficulty && enemies.Count < 7)
        {
            var maximum = difficulty - currentDifficulty;
            var nextEnemy = possible.ToList().FindAll(
                enemy => enemy.PowerLevel <= maximum
            ).Random();
            enemies.Add(nextEnemy);
            currentDifficulty = currentDifficulty + nextEnemy.PowerLevel;
        }
        return enemies;
    }
}
