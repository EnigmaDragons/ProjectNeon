
public abstract class NonMapAutoStartSegment : StageSegment
{
    public override bool ShouldCountTowardsEnemyPowerLevel => false;
    public override bool ShouldSpawnThisOnMap(CurrentAdventureProgress p) => false;
    public override MapNodeType MapNodeType => MapNodeType.Unknown;
    public override Maybe<string> Corp => Maybe<string>.Missing();
    public override bool ShouldAutoStart => true;
}