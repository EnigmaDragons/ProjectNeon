using UnityEngine;

public class StartNextStageHandler : OnMessage<StartNextStage>
{
    [SerializeField] private AdventureProgress adventure;
    
    protected override void Execute(StartNextStage msg)
    {
        adventure.CurrentStageSegment.Start();
    }
}
