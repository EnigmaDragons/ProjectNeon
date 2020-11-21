using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class CurrentGameMap2 : ScriptableObject
{
    public GameMap2 Map { get; private set; }
    public bool IsMapGenerated { get; private set; }
    public MapNode GeneratedMap { get; private set; }
    public int CurrentPositionId { get; set; }
    
    private Dictionary<int, MapNode> _map;
    private int _bossNodeId;

    public void SetMap(GameMap2 map)
    {
        Map = map;
        IsMapGenerated = false;
    }

    public void SetupMap(MapNode generatedMap)
    {
        GeneratedMap = generatedMap;
        CurrentPositionId = generatedMap.NodeId;
    }

    public MapNode GetMapNode(int id)
    {
        InitMapIndexIfNeeded();
        return _map[id];
    }
    
    public MapNode GetBossNode()
    {
        InitMapIndexIfNeeded();
        return _map[_bossNodeId];
    }
    
    private void InitMapIndexIfNeeded()
    {
        if (_map != null)
            return;
        _map = new Dictionary<int, MapNode>();
        RecursivelyGenerateMap(GeneratedMap);
    }
    
    private void RecursivelyGenerateMap(MapNode node)
    {
        _map[node.NodeId] = node;
        if (node.Children.Any())
            node.Children.ForEach(RecursivelyGenerateMap);
        else
            _bossNodeId = node.NodeId;
    } 
}