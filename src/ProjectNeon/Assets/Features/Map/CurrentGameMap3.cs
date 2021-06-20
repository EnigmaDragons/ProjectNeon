using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Maps/Current Game Map 3")]
public class CurrentGameMap3 : ScriptableObject
{
    public GameMap3 CurrentMap { get; set; }
    public List<MapNodeType> CompletedNodes { get; set; }
    public Vector2 CurrentPosition { get; set; }
    public List<MapNode3> CurrentChoices { get; set; }
    
    public void SetMap(GameMap3 map)
    {
        CurrentMap = map;
    }
}