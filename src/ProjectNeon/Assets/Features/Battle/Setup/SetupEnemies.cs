using UnityEngine;

public class SetupEnemies : MonoBehaviour
{
    [SerializeField] private EnemyArea enemyArea;
    [SerializeField] private EncounterBuilder encounterBuilder;
    [SerializeField] private GameEvent onEncounterGenerated;
    [SerializeField] private GameEvent onEnemyTurnsConfirmed;
    [SerializeField] private CardResolutionZone resolutionZone;

    void Start()
    {
        enemyArea.enemies = encounterBuilder.Generate();
        onEncounterGenerated.Publish();
    }

    void ChoseCards()
    {
        enemyArea.enemies.ForEach(
            enemy => resolutionZone.AddEnemyMove(enemy.turn.Play())
        );
        onEnemyTurnsConfirmed.Publish();
    }
}
