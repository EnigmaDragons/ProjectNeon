using UnityEngine;

public class AutoProgression : OnMessage<NodeFinished>
{
    [SerializeField] private AdventureProgressV4 progress;

    private void Start() => progress.CurrentStageSegment.Start();
    protected override void Execute(NodeFinished msg)
    {
        progress.Advance();
        progress.CurrentStageSegment.Start();
    }
}