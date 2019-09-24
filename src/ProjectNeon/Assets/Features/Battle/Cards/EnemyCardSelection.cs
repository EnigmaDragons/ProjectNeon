using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCardSelection : MonoBehaviour
{
    [SerializeField] private CardResolutionZone resolutionZone;
    [SerializeField] private GameEvent onEnemyTurnsConfirmed;
    [SerializeField] private BattleState battle;


    private List<Enemy> enemies = new List<Enemy>();
    /**
     * @todo #190:30min Refactor the relation Member - Enemy. At the moment we can't extract
     *  enemies from BattleState object because they are all wrapped in Member instances.
     */

    void ChooseCards()
    {
        enemies.ForEach(
            enemy => resolutionZone.Add(enemy.turn.Play())
        );
        onEnemyTurnsConfirmed.Publish();
    }
}
