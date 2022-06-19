using UnityEngine;

public class PunchYourselfHandler : OnMessage<PunchYourself>
{
    [SerializeField] private string refName;
    
    protected override void Execute(PunchYourself msg)
    {
        if (msg.Name == refName)
            Message.Publish(new TweenMovementRequested(transform, new Vector3(1.5f, 1.5f, 1), 1, MovementDimension.Scale, TweenMovementType.AutoRubberBand, ""));
    }
}