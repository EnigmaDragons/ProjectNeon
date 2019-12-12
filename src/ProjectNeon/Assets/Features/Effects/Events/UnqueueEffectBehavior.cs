using UnityEngine;

public class UnqueueEffectBehavior : MonoBehaviour
{
    [SerializeField] private BattleState battleState;

    void OnEnable()
    {
        BattleEvent.Subscribe<RemoveEffectFromQueue>((msg) => battleState.QueuedEffects.Remove(msg.Effect), this);
    }

    void OnDisable()
    {
        BattleEvent.Unsubscribe(this);
    }
}
