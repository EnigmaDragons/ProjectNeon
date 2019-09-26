using UnityEngine;

public class SetupEnemies : MonoBehaviour
{
    [SerializeField] private EnemyArea enemyArea;
    [SerializeField] private EncounterBuilder encounterBuilder;
    [SerializeField] private GameEvent onEncounterGenerated;
    
    void Start()
    {
        enemyArea = enemyArea.Initialized(encounterBuilder.Generate());
        onEncounterGenerated.Publish();
    }
}
