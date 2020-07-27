using UnityEngine;

public class OnStartNextStage : OnMessage<StartNextStage>
{
    [SerializeField] private AdventureProgress adventure;
    
    protected override void Execute(StartNextStage msg)
    {
        adventure.CurrentStageSegment.Start();
    }
}
