using System.Collections.Generic;
using UnityEngine;

public class EncounterBuilder : MonoBehaviour
{
    [SerializeField]
    private List<Enemy> possible;

    [SerializeField, Range(1,10)]
    private int difficulty;

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
        List<Enemy> enemies = new List<Enemy>();
        possible.FindAll(
            enemy => enemy.powerLevel <= difficulty
        ).ForEach(
            enemy => enemies.Add(enemy)
        );
        return enemies;
    }
}
