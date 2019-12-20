using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountSpellHitBehavior : MonoBehaviour
{
    [SerializeField] private CardResolutionZone resolutionZone;

    void OnEnable()
    {
        BattleEvent.Subscribe<SpellPerformed>((msg) => resolutionZone.SpellHitsOnLastCard++ , this);
    }

    void OnDisable()
    {
        BattleEvent.Unsubscribe(this);
    }
}
