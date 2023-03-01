using UnityEngine;

public class ProgressionHandlerV5 : OnMessage<NodeFinished>
{
    [SerializeField] private AdventureProgressV5 progress;
    [SerializeField] private CurrentMapSegmentV5 map;
    [SerializeField] private BoolVariable skippingStory;
    
    [SerializeField] private CurrentAdventureProgress adventureProgress;
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private Navigator navigator;
    [SerializeField] private AdventureConclusionState conclusion;
    [SerializeField] private PartyAdventureState partyState;

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
        if (progress.CurrentStageSegment.ShouldAutoStart && progress.CurrentStageSegment.MapNodeType == MapNodeType.MainStory && skippingStory.Value)
        {
            if (progress.IsFinalStageSegment)
            {
                GameWrapup.NavigateToVictoryScreen(adventureProgress, adventure, navigator, conclusion, partyState.Heroes);
                return;
            }
            map.CompleteCurrentNode();
            progress.Advance();
            map.ClearSegment();
            Go();
            return;
        }

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
