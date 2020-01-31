using UnityEngine;

public sealed class ResolveAllCards : MonoBehaviour
{
    [SerializeField] private CardResolutionZone resolutionZone;
    [SerializeField] private GameEvent trigger;
    [SerializeField] private FloatReference delay = new FloatReference(1.5f);

    private void OnEnable() => trigger.Subscribe(Execute, this);
    private void OnDisable() => trigger.Unsubscribe(this);

    private void Execute()
    {
        resolutionZone.Resolve(this, delay);
    }
}
