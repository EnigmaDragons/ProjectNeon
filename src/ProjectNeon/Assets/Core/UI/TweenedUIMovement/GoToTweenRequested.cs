using UnityEngine;

public class GoToTweenRequested
{
    public Transform Transform { get; }
    public Vector3 Destination { get; }
    public int Seconds { get; }

    public GoToTweenRequested(Transform transform, Vector3 destination, int seconds)
    {
        Transform = transform;
        Destination = destination;
        Seconds = seconds;
    }
}