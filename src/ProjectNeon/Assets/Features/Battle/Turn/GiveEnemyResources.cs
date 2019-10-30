using UnityEngine;

public class GiveEnemyResources : MonoBehaviour
{
    [SerializeField] private BattleState battleState;
    [SerializeField] private GameEvent onTurnWrapUpStarted;
    [SerializeField] private GameEvent onFinished;
    [SerializeField] private int numToGive;

    private void OnEnable() => onTurnWrapUpStarted.Subscribe(Execute, this);

    private void OnDisable() => onTurnWrapUpStarted.Unsubscribe(this);

    private void Execute()
    {
        battleState.Enemies.ForEach(e => e.State.GainPrimaryResource(numToGive));
        onFinished.Publish();
    }
}
