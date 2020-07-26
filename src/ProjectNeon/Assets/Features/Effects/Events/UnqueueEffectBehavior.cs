using UnityEngine;

public class UnqueueEffectBehavior : MonoBehaviour
{
    [SerializeField] private BattleState battleState;

    void OnEnable()
    {
        Message.Subscribe<RemoveEffectFromQueue>((msg) => battleState.QueuedEffects.Remove(msg.Effect), this);
    }

    void OnDisable()
    {
        Message.Unsubscribe(this);
    }
}
