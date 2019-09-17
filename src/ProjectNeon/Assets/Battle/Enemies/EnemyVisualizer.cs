using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisualizer : MonoBehaviour
{

    [SerializeField] private EnemyArea enemyArea;
    [SerializeField] private List<GameObject> enemies;

    /**
     * @todo #29:15min Draw enemy characters on battle screen. Enemy positioning must be relative to 
     *  total enemy count. Each enemy must dinamically create a sprite inside EnemyArea Object.
     */
    /**
     * @todo #29:30min Wire enemy setup with correct events. Enemy setup on battle scene must be triggered
     *  upon BattleSetupStarted event and broadcast BattleSetupEnemyPartyEntered event after enemy party
     *  is ready.
     */

    void Start()
    {
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<SpriteRenderer>().sprite = enemyArea.enemies[enemies.IndexOf(enemy)].image;
        }
    }
}