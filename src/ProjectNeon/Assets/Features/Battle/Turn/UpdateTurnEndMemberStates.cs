using UnityEngine;

public class UpdateTurnEndMemberStates : MonoBehaviour
{
    [SerializeField] private BattleState battleState;
    [SerializeField] private GameEvent onReady;
    [SerializeField] private GameEvent onFinished;

    private void OnEnable() => onReady.Subscribe(Execute, this);
    private void OnDisable() => onReady.Unsubscribe(this);

    private void Execute()
    {
        battleState.Members.Values.CopiedForEach(m => m.State.AdvanceTurn());
        onFinished.Publish();
    }
}
