using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class CurrentGameMap2 : ScriptableObject
{
    public GameMap2 Map { get; private set; }
    public bool IsMapGenerated { get; private set; }
    public List<MapNode> GeneratedMap { get; private set; }
    public string CurrentPositionId { get; set; }
    
    private Dictionary<string, MapNode> _map;

    public void SetMap(GameMap2 map)
    {
        Map = map;
        IsMapGenerated = false;
    }

    public void SetupMap(List<MapNode> generatedMap)
    {
        GeneratedMap = generatedMap;
        IsMapGenerated = true;
        CurrentPositionId = generatedMap.First().NodeId;
        _map = new Dictionary<string, MapNode>();
        GeneratedMap.ForEach(x => _map[x.NodeId] = x);
    }

    public MapNode GetMapNode(string id) => _map[id];
}