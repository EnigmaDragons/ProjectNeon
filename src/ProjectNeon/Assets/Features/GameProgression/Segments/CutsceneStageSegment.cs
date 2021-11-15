
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Cutscene")]
public class CutsceneStageSegment : StageSegment
{
    [SerializeField] private Cutscene cutscene;

    public override string Name { get; } = "Cutscene";
    public override void Start() => Message.Publish(new StartCutsceneRequested(cutscene));
    public override Maybe<string> Detail { get; } = Maybe<string>.Missing();
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;
}
