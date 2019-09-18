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

public class EncounterBuilder : MonoBehaviour
{
    [SerializeField] private Enemy[] possible;

    [SerializeField, Range(1, 10)] private int difficulty;

    public void Init(IEnumerable<Enemy> possibleEnemies, int newDifficulty)
    {
        possible = possibleEnemies.ToArray();
        difficulty = newDifficulty;
    }
    
    public List<Enemy> Generate()
    {
        /**
         * @todo #52:30min Evolve Encounter Generation after playtesting. 
         */
        /**
         * @todo #52:15min After #29 merge, wire encounter builder restult with BattleSetup so we can use the generated
         *  enemies into the battle scene. BattleEnemiesSpawned  event must be triggered after they are added to
         *  the scene.
         */
        int currentDifficulty = 0;
        List<Enemy> enemies = new List<Enemy>();

        while (currentDifficulty < difficulty && enemies.Count < 7)
        {
            int maximum = difficulty - currentDifficulty;
            var nextEnemy = possible.ToList().FindAll(
                enemy => enemy.powerLevel <= maximum
            ).Random();
            enemies.Add(nextEnemy);
            currentDifficulty = currentDifficulty + nextEnemy.powerLevel;
        }
        return enemies;
    }
}
