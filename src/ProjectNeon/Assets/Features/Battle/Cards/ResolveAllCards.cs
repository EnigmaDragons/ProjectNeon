using UnityEngine;

public sealed class ResolveAllCards : MonoBehaviour
{
    [SerializeField] private CardResolutionZone resolutionZone;
    [SerializeField] private GameEvent trigger;

    private void OnEnable() => trigger.Subscribe(Execute, this);
    private void OnDisable() => trigger.Unsubscribe(this);

    private void Execute()
    {
        resolutionZone.Resolve(this);
    }
}
