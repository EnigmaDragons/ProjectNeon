using UnityEngine;

public class ProgressionHandlerV5 : OnMessage<NodeFinished>
{
    [SerializeField] private AdventureProgressV5 progress;
    [SerializeField] private CurrentMapSegmentV5 map;

    private void Start() => Go();

    protected override void Execute(NodeFinished msg)
    {
        map.CompleteCurrentNode();
        if (map.CurrentNode.IsMissingOr(c => c.AdvancesAdventure))
        {
            progress.Advance();
            map.AdvanceToNextSegment();
        }
        Go();
    }
    
    private void Go()
    {
        Message.Publish(new RegenerateMapRequested());
        if (progress.CurrentStageSegment.ShouldAutoStart)
            progress.CurrentStageSegment.Start();
    }
}