using UnityEngine;

public class StopMovementTweeningRequested
{
    public Transform Transform { get; }
    public MovementDimension Dimension { get; }

    public StopMovementTweeningRequested(Transform transform, MovementDimension dimension)
    {
        Transform = transform;
        Dimension = dimension;
    }
}