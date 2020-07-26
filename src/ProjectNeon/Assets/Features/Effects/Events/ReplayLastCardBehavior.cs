using UnityEngine;

public class ReplayLastCardBehavior : MonoBehaviour
{
    [SerializeField] private CardResolutionZone cardResolutionZone;

    void OnEnable()
    {
        Message.Subscribe<ReplayLastCard>(_ => cardResolutionZone.LastPlayed.Perform(), this);
    }

    void OnDisable()
    {
        Message.Unsubscribe(this);
    }
}
