using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCardSelection : MonoBehaviour
{
    [SerializeField] private CardResolutionZone resolutionZone;
    [SerializeField] private GameEvent onEnemyTurnsConfirmed;

    private List<Enemy> enemies;

    void ChoseCards()
    {
        enemies.ForEach(
            enemy => resolutionZone.Add(enemy.turn.Play())
        );
        onEnemyTurnsConfirmed.Publish();
    }
}
