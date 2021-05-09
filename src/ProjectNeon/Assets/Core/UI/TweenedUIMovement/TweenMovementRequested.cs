using UnityEngine;

public class TweenMovementRequested
{
    public Transform Transform { get; }
    public Vector3 RelativeDistance { get; }
    public float Seconds { get; }
    public MovementDimension Dimension { get; }
    public TweenMovementType MovementType { get; }
    public string MovementName { get; }
    public bool UseScaledTime { get; set; } = true;

    public TweenMovementRequested(Transform transform, Vector3 relativeDistance, float seconds, MovementDimension dimension)
        : this(transform, relativeDistance, seconds, dimension, TweenMovementType.GoTo, "") {}

    public TweenMovementRequested(Transform transform, Vector3 relativeDistance, float seconds, MovementDimension dimension, TweenMovementType movementType, string movementName)
    {
        Transform = transform;
        RelativeDistance = relativeDistance;
        Seconds = seconds;
        Dimension = dimension;
        MovementType = movementType;
        MovementName = movementName;
    }
}