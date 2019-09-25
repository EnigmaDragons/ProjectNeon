using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCardSelection : MonoBehaviour
{
    [SerializeField] private CardResolutionZone resolutionZone;
    [SerializeField] private GameEvent onEnemyTurnsConfirmed;
    [SerializeField] private BattleState battle;

    void ChooseCards()
    {
        this.battle.GetEnemies().ForEach(
            enemy => resolutionZone.Add(enemy.Player.Play())
        );
        onEnemyTurnsConfirmed.Publish();
    }
}
