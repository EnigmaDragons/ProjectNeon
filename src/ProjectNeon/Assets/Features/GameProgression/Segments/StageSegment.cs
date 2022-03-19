using UnityEngine;

public abstract class StageSegment : ScriptableObject, IStageSegment
{
    public abstract string Name { get; }
    public abstract bool ShouldCountTowardsEnemyPowerLevel { get; }
    public abstract bool ShouldAutoStart { get; }
    public abstract void Start();
    public abstract Maybe<string> Detail { get; }
    public abstract MapNodeType MapNodeType { get; }
    public abstract Maybe<string> Corp { get; }
    public abstract IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData);
}
