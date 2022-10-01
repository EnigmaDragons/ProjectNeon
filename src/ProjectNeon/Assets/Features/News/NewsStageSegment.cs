using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/NewsCast")]
public class NewsStageSegment : StageSegment
{
    [SerializeField] private Cutscene cutscene;
    
    public override string Name { get; } = "NewsCast";
    public override bool ShouldCountTowardsEnemyPowerLevel { get; } = false;
    public override void Start() => Message.Publish(new ShowNewscast(cutscene));
    public override Maybe<string> Detail { get; } = Maybe<string>.Missing();
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;
    public override bool ShouldSpawnThisOnMap(CurrentAdventureProgress p) => false;
    public override MapNodeType MapNodeType { get; } = MapNodeType.Unknown;
    public override Maybe<string> Corp { get; } = Maybe<string>.Missing();
    public override bool ShouldAutoStart { get; } = true;
    
    public Cutscene Cutscene => cutscene;
}
