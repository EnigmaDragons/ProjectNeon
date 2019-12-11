using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueEffectBehavior : MonoBehaviour
{
    [SerializeField] private BattleState battleState;

    void OnEnable()
    {
        BattleEvent.Subscribe<AddEffectToQueue>((msg) => battleState.QueuedEffects.Add(msg.Effect), this);
    }

    void OnDisable()
    {
        BattleEvent.Unsubscribe(this);
    }
}
