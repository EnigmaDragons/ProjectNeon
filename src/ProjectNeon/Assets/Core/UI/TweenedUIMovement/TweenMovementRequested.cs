using UnityEngine;

public class TweenMovementRequested
{
    public Transform Transform { get; }
    public Vector3 RelativeDistance { get; }
    public float Seconds { get; }
    public TweenMovementType MovementType { get; }
    public string MovementName { get; }

    public TweenMovementRequested(Transform transform, Vector3 relativeDistance, float seconds)
        : this(transform, relativeDistance, seconds, TweenMovementType.GoTo, "") {}

    public TweenMovementRequested(Transform transform, Vector3 relativeDistance, float seconds, TweenMovementType movementType, string movementName)
    {
        Transform = transform;
        RelativeDistance = relativeDistance;
        Seconds = seconds;
        MovementType = movementType;
        MovementName = movementName;
    }
}