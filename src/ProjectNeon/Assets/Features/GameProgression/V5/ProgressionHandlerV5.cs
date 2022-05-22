using UnityEngine;

public class ProgressionHandlerV5 : OnMessage<NodeFinished>
{
    [SerializeField] private AdventureProgressV5 progress;
    [SerializeField] private CurrentMapSegmentV5 map;

    private void Start()
    {
        Go();
    }

    protected override void Execute(NodeFinished msg)
    {
        map.CompleteCurrentNode();
        if (map.CurrentNode.IsMissingOr(c => c.AdvancesAdventure))
        {
            Log.Info("V5 - Advancing Segment");
            progress.Advance();
            map.AdvanceToNextSegment();
        }
        Go();
    }
    
    private void Go()
    {
        Log.Info("V5 - Regenerating Map");
        Message.Publish(new RegenerateMapRequested());
        Message.Publish(new AutoSaveRequested());
        if (progress.CurrentStageSegment.ShouldAutoStart)
        {
            Log.Info($"V5 - Auto-Start Segment");
            map.CompleteCurrentNode();
            progress.CurrentStageSegment.Start();
            map.ClearSegment();
        }
    }
}