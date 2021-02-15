using UnityEngine;

public class GoToTweenRequested
{
    public Transform Transform { get; }
    public Vector3 Destination { get; }
    public float Seconds { get; }

    public GoToTweenRequested(Transform transform, Vector3 destination, float seconds)
    {
        Transform = transform;
        Destination = destination;
        Seconds = seconds;
    }
}