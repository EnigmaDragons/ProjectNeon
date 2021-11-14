using UnityEngine;

public class AutoProgression : OnMessage<NodeFinished>
{
    [SerializeField] private AdventureProgressV4 progress;

    private void Start() => PlayNextSegment();
    protected override void Execute(NodeFinished msg) => PlayNextSegment();

    private void PlayNextSegment()
    {
        progress.CurrentStageSegment.Start();
    }
}