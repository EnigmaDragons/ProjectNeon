using System;
using Features.GameProgression.Messages;
using UnityEngine;

[Obsolete("MapView1")]
public class StartNextStageHandler : OnMessage<StartNextStage>
{
    [SerializeField] private AdventureProgress adventure;
    
    protected override void Execute(StartNextStage msg)
    {
        Message.Publish(new AutoSaveRequested());
        adventure.CurrentStageSegment.Start();
    }
}
