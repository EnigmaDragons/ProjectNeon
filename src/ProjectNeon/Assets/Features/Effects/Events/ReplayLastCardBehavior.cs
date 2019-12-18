using UnityEngine;

public class ReplayLastCardBehavior : MonoBehaviour
{
    [SerializeField] private BattleState battleState;

    void OnEnable()
    {
        BattleEvent.Subscribe<ReplayLastCard>(_ => battleState.LastPlayed.Perform(), this);
    }

    void OnDisable()
    {
        BattleEvent.Unsubscribe(this);
    }
}
