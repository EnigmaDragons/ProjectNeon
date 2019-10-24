using UnityEngine;

public abstract class StageSegment : ScriptableObject
{
    public abstract string Name { get; }
    public abstract void Start();
}
