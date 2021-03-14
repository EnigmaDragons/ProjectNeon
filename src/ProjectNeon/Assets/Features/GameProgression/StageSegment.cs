using UnityEngine;

public abstract class StageSegment : ScriptableObject, IStageSegment
{
    public abstract string Name { get; }
    public abstract void Start();
    public abstract Maybe<string> Detail { get; }
    public abstract IStageSegment GenerateDeterministic(AdventureGenerationContext ctx);
}
