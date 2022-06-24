using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/DraftSegment")]
public class DraftSegment : NonMapAutoStartSegment
{
    public override string Name { get; } = "Draft";
    public override void Start() => Message.Publish(new BeginDraft());

    public override Maybe<string> Detail { get; } = Maybe<string>.Missing();
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;
}
