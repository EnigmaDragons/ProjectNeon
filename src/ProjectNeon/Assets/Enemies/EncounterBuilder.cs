using System.Collections.Generic;
using UnityEngine;

public class EncounterBuilder : MonoBehaviour
{
    [SerializeField]
    private List<Enemy> possible;

    [SerializeField]
    private int difficulty;

    public List<Enemy> generate()
    {
        /**
         * @todo #52:30min Implement encounter generation after the encounter algorithm is defined, and then
         *  return correct list of enemies in generate() method.
         */
        /**
         * @todo #52:15min After #29 merge, wire encounter builder restult with BattleSetup so we can use the generated
         *  enemies into the battle scene. BattleEnemiesSpawned  event must be triggered after they are added to
         *  the scene.
         */
        if (difficulty < 0) difficulty = 1;
        if (difficulty > 7) difficulty = 7;
        List<Enemy> enemies = new List<Enemy>();
        for (int i = 0; i < difficulty; i++)
        {
            enemies.Add(possible[Random.Range(0, possible.Count-1)]); ;
        }
        return possible;
    }
}
