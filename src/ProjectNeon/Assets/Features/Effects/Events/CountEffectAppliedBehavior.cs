using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountEffectAppliedBehavior : MonoBehaviour
{
    [SerializeField] private CardResolutionZone resolutionZone;

    void OnEnable()
    {
        BattleEvent.Subscribe<EffectPerformed>((msg) => resolutionZone.SpellHitsOnLastCard++ , this);
    }

    void OnDisable()
    {
        BattleEvent.Unsubscribe(this);
    }
}
