using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueEffectBehavior : MonoBehaviour
{
    [SerializeField] private BattleState battleState;

    void OnEnable()
    {
        Message.Subscribe<AddEffectToQueue>((msg) => battleState.QueuedEffects.Add(msg.Effect), this);
    }

    void OnDisable()
    {
        Message.Unsubscribe(this);
    }
}
