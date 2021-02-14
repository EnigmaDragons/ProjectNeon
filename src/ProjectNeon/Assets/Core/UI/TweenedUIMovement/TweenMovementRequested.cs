using UnityEngine;

public class TweenMovementRequested
{
    public Transform Transform { get; }
    public Vector3 RelativeDistance { get; }
    public float Seconds { get; }

    public TweenMovementRequested(Transform transform, Vector3 relativeDistance, float seconds)
    {
        Transform = transform;
        RelativeDistance = relativeDistance;
        Seconds = seconds;
    }
}