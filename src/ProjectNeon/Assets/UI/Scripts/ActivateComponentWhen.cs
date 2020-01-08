using UnityEngine;

public sealed class ActivateComponentWhen : MonoBehaviour
{
    [ReadOnly, SerializeField] private bool isActive;
    [SerializeField] private GameEvent[] activateOn;
    [SerializeField] private GameEvent[] deactivateOn;
    [SerializeField] private bool startsActive = false;
    [SerializeField] private MonoBehaviour target;
    
    private void OnEnable()
    {
        isActive = startsActive;
        target.enabled = isActive;
        activateOn.ForEach(e => e.Subscribe(() => SetTargetState(true), this));
        deactivateOn.ForEach(e => e.Subscribe(() => SetTargetState(false), this));
    }

    private void OnDisable()
    {
        activateOn.ForEach(e => e.Unsubscribe(this));
        deactivateOn.ForEach(e => e.Unsubscribe(this));
    }

    void SetTargetState(bool active)
    {
        isActive = active;
        target.enabled = isActive;
    }
}
