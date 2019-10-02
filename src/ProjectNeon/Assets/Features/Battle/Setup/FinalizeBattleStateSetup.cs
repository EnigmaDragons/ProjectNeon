using UnityEngine;

public class FinalizeBattleStateSetup : MonoBehaviour
{
    [SerializeField] private GameEvent onComplete;
    [SerializeField] private BattleState battleState;

    public void FinalizeSetup()
    {
        battleState.Init();
        onComplete.Publish();
    }
}
