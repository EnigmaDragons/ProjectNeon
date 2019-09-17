using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisualizer : MonoBehaviour
{

    [SerializeField] private EnemyArea enemyArea;
    [SerializeField] private GameObject enemy1;
    [SerializeField] private GameObject enemy2;
    [SerializeField] private GameObject enemy3;
    [SerializeField] private GameObject enemy4;
    [SerializeField] private GameObject enemy5;

    /**
     * @todo #28:15min Render player characters in battle screen.
     */

    void Start()
    {
        enemy1.GetComponent<SpriteRenderer>().sprite = enemyArea.enemies[0].image;
        enemy2.GetComponent<SpriteRenderer>().sprite = enemyArea.enemies[1].image;
        enemy3.GetComponent<SpriteRenderer>().sprite = enemyArea.enemies[2].image;
        enemy4.GetComponent<SpriteRenderer>().sprite = enemyArea.enemies[3].image;
        enemy5.GetComponent<SpriteRenderer>().sprite = enemyArea.enemies[4].image;
    }
}