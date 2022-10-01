
using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Cutscene")]
public class CutsceneStageSegment : StageSegment
{
    [SerializeField] private Cutscene cutscene;
    [SerializeField] private bool shouldAutoStart;

    public override string Name { get; } = "Cutscene";
    public override bool ShouldAutoStart => shouldAutoStart;
    public override bool ShouldCountTowardsEnemyPowerLevel => false;
    public override void Start() => Message.Publish(new StartCutsceneRequested(cutscene, Maybe<Action>.Missing()));
    public override Maybe<string> Detail { get; } = Maybe<string>.Missing();
    public override MapNodeType MapNodeType => MapNodeType.MainStory;
    public override Maybe<string> Corp => Maybe<string>.Missing();
    
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;
    public override bool ShouldSpawnThisOnMap(CurrentAdventureProgress p) 
        => cutscene.Segments.Any(s => s.ShouldShow(x => p.AdventureProgress.IsTrue(x)));

    public Cutscene Cutscene => cutscene;
}
