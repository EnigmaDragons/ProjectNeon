using Features.GameProgression.Messages;
using UnityEngine;

public class StartNextStageHandler : OnMessage<StartNextStage>
{
    [SerializeField] private AdventureProgress adventure;
    
    protected override void Execute(StartNextStage msg)
    {
        Message.Publish(new AutoSaveRequested());
        adventure.CurrentStageSegment.Start();
    }
}
