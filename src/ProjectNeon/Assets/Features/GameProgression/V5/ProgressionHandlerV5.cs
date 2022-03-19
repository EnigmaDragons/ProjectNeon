using UnityEngine;

public class ProgressionHandlerV5 : OnMessage<NodeFinished>
{
    [SerializeField] private AdventureProgressV5 progress;

    private void Start() => Go();

    protected override void Execute(NodeFinished msg)
    {
        progress.Advance();
        Go();
    }
    
    private void Go()
    {
        if (progress.CurrentStageSegment.ShouldAutoStart)
            progress.CurrentStageSegment.Start();
        else
            Message.Publish(new RegenerateMapRequested());
    }
}