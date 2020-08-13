using System;
using UnityEngine;

public sealed class ActivateObjectWhen : MonoBehaviour
{
    [ReadOnly, SerializeField] private bool isActive;
    [SerializeField] private GameEvent[] activateOn;
    [SerializeField] private GameEvent[] deactivateOn;
    [SerializeField] private bool startsActive = false;
    [SerializeField] private GameObject target;
    
    private void OnEnable()
    {
        try
        {
            isActive = startsActive;
            target.SetActive(isActive);
            activateOn.ForEach(e => e.Subscribe(() => SetTargetState(true), this));
            deactivateOn.ForEach(e => e.Subscribe(() => SetTargetState(false), this));
        }
        catch (Exception e)
        {
            Log.Error(e.Message, this);
        }
    }

    private void OnDisable()
    {
        activateOn.ForEach(e => e.Unsubscribe(this));
        deactivateOn.ForEach(e => e.Unsubscribe(this));
    }

    void SetTargetState(bool active)
    {
        isActive = active;
        target.SetActive(isActive);
    }
}
