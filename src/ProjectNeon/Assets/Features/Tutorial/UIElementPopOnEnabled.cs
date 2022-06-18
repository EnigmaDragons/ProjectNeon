using UnityEngine;

public class UIElementPopOnEnabled : MonoBehaviour
{
    private const float _seconds = 1f;
    private const float _scale = 1.5f;

    private void OnEnable()
    {
        if (Time.timeSinceLevelLoad > 1)
            Message.Publish(new TweenMovementRequested(transform, new Vector3(_scale, _scale, 0), _seconds, MovementDimension.Scale, TweenMovementType.RubberBand, "")); 
    }
}