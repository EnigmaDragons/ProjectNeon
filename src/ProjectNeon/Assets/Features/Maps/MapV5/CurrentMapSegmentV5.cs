using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Maps/Current Map Segment V5")]
public class CurrentMapSegmentV5 : ScriptableObject
{
    public GameMap3 CurrentMap { get; set; }
    public Maybe<MapNode3> CurrentNode { get; set; } = Maybe<MapNode3>.Missing();
    public List<MapNode3> CurrentChoices { get; set; } = new List<MapNode3>();
    public Vector2 PreviousPosition { get; set; } = Vector2.zero;
    public Vector2 DestinationPosition { get; set; } = Vector2.zero;
    public OneTimeSeed CurrentNodeRngSeed { get; set; } = ConsumableRngSeed.GetValid();
    public bool IncludeCurrentNodeInSaveData { get; set; }

    public void CompleteCurrentNode()
    {
        if (CurrentNode.IsMissing)
            return;

        CurrentChoices.Remove(CurrentNode.Value);
        PreviousPosition = DestinationPosition;
    }

    public void DisableSavingCurrentNode()
    {
        IncludeCurrentNodeInSaveData = false;
        PreviousPosition = DestinationPosition;
    }

    public void AdvanceToNextSegment() => ClearSegment();

    public void SetMap(GameMap3 map)
    {
        CurrentMap = map;
        PreviousPosition = map.StartingPoint;
        DestinationPosition = map.StartingPoint;
        ClearSegment();
    }

    public void ClearSegment()
    {
        PreviousPosition = CurrentMap.StartingPoint;
        DestinationPosition = CurrentMap.StartingPoint;
        CurrentNode = Maybe<MapNode3>.Missing();
        CurrentChoices = new List<MapNode3>();
        IncludeCurrentNodeInSaveData = false;
        UpdateSeed();
    }

    private void UpdateSeed() => CurrentNodeRngSeed = ConsumableRngSeed.GetValid();
}
