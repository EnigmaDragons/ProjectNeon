using UnityEngine;

public class SnapBackTweenRequested
{
    public Transform Transform { get; }
    public string MovementName { get; }

    public SnapBackTweenRequested(Transform transform, string movementName)
    {
        Transform = transform;
        MovementName = movementName;
    }
}