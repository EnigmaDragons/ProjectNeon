﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Maps/Current Game Map 3")]
public class CurrentGameMap3 : ScriptableObject
{
    public GameMap3 CurrentMap { get; set; }
    public List<MapNodeType> CompletedNodes { get; set; } = new List<MapNodeType>();
    public Vector2 CurrentPosition { get; set; } = Vector2.zero;
    public List<MapNode3> CurrentChoices { get; set; } = new List<MapNode3>();

    public void SetMap(GameMap3 map)
    {
        CurrentMap = map;
        CompletedNodes = new List<MapNodeType>();
        CurrentPosition = map.StartingPoint;
        CurrentChoices = new List<MapNode3>();
    }

    public int Progress => CompletedNodes?.Count(x => x != MapNodeType.StoryEvent) ?? 0;
}