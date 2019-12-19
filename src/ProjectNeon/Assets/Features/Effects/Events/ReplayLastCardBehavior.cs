using UnityEngine;

public class ReplayLastCardBehavior : MonoBehaviour
{
    [SerializeField] private CardResolutionZone cardResolutionZone;

    void OnEnable()
    {
        BattleEvent.Subscribe<ReplayLastCard>(_ => cardResolutionZone.LastPlayed.Perform(), this);
    }

    void OnDisable()
    {
        BattleEvent.Unsubscribe(this);
    }
}
