using UnityEngine;

public class UnqueueEffectBehavior : MonoBehaviour
{
    [SerializeField] private BattleState battleState;

    void OnEnable()
    {
        BattleEvent.Subscribe<RemoveEffectFromQueue>((effect) => battleState.QueuedEffects.Remove(effect.Effect), this);
    }

    void OnDisable()
    {
        BattleEvent.Unsubscribe(this);
    }
}
