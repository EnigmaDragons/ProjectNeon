using UnityEngine;

public class EnemyCardSelection : MonoBehaviour
{
    [SerializeField] private CardResolutionZone resolutionZone;
    [SerializeField] private GameEvent onEnemyTurnsConfirmed;
    [SerializeField] private BattleState battle;
    [SerializeField] private EnemyArea enemyArea;
 
    void ChooseCards()
    {
        enemyArea.Enemies.ForEach(
            enemy => resolutionZone.Add(enemy.AI.Play(enemy))
        );
        onEnemyTurnsConfirmed.Publish();
    }
}
