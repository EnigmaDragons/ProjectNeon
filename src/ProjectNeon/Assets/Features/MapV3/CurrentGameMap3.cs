using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Maps/Current Game Map 3")]
public class CurrentGameMap3 : ScriptableObject
{
    public GameMap3 CurrentMap { get; set; }
    public Maybe<MapNode3> CurrentNode { get; set; } = Maybe<MapNode3>.Missing();
    public List<MapNode3> CompletedNodes { get; set; } = new List<MapNode3>();
    public Vector2 PreviousPosition { get; set; } = Vector2.zero;
    public Vector2 DestinationPosition { get; set; } = Vector2.zero;
    public List<MapNode3> CurrentChoices { get; set; } = new List<MapNode3>();
    public bool HasCompletedEventEnRoute { get; set; }
    public int CurrentNodeRngSeed { get; set; } = Guid.NewGuid().GetHashCode();

    public void SetMap(GameMap3 map)
    {
        CurrentMap = map;
        CurrentNode = Maybe<MapNode3>.Missing();
        CompletedNodes = new List<MapNode3>();
        PreviousPosition = map.StartingPoint;
        DestinationPosition = map.StartingPoint;
        CurrentChoices = new List<MapNode3>();
        HasCompletedEventEnRoute = false;
        UpdateSeed();
    }

    public int Progress => CompletedNodes?.Count ?? 0;

    public void CompleteCurrentNode()
    {
        if (CurrentNode.IsMissing || CurrentNode.Value.Type == MapNodeType.Start)
            return;
        CompletedNodes.Add(CurrentNode.Value);
        CurrentChoices = new List<MapNode3>();
        PreviousPosition = DestinationPosition;
        CurrentNode = Maybe<MapNode3>.Missing();
        UpdateSeed();
    }
    
    private void UpdateSeed() => CurrentNodeRngSeed = Guid.NewGuid().GetHashCode();
}