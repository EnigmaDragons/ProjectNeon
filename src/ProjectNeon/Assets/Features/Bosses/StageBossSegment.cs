using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/StageBossSegment")]
public class StageBossSegment : StageSegment
{
    [SerializeField] public int stage;
    [SerializeField] public CurrentBoss boss;

    private StageSegment Segment => boss.Boss.Stage(stage);
    
    public override string Name => Segment.Name;
    public override bool ShouldCountTowardsEnemyPowerLevel => Segment.ShouldCountTowardsEnemyPowerLevel;
    public override void Start() => Segment.Start();
    public override Maybe<string> Detail => Segment.Detail;
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => Segment.GenerateDeterministic(ctx, mapData);
    public override bool ShouldSpawnThisOnMap(CurrentAdventureProgress p) => Segment.ShouldSpawnThisOnMap(p);
    public override MapNodeType MapNodeType => Segment.MapNodeType;
    public override Maybe<string> Corp => Segment.Corp;
    public override bool ShouldAutoStart => Segment.ShouldAutoStart;
}