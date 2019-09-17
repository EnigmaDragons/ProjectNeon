using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Puts the enemy party into screen.
 * 
 * Display enemies into screen in the following format (each number is enemies variable index):
 * 0 2 4 6
 *  1 3 5
 * Uses enemy sprites with default size of 100 x 200
 */
public class EnemyVisualizer : MonoBehaviour
{

    [SerializeField] private EnemyArea enemyArea;
    [SerializeField] private List<GameObject> enemies;

    //enemy sprite default height
    private int height = 200;

    //enemy sprite default width
    private int width = 100;

    /**
      * @todo #29:5min Wire enemy setup with correct events. Enemy setup on battle scene must be triggered
      *  upon BattleSetupStarted event and broadcast BattleSetupEnemyPartyEntered event after enemy party
      *  is ready.
      */

    void Start()
    {
        for (int i= 0; i < enemies.Count; i++)
        {
            GameObject enemy = enemies[i];
            SpriteRenderer renderer = enemy.AddComponent<SpriteRenderer>();
            renderer.sprite = enemyArea.enemies[i].image;
            renderer.transform.position = new Vector3Int(i * width, (i % 2) * height, 0);
        }
    }
}