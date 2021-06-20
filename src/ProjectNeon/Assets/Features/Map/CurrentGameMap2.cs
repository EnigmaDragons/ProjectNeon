using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class CurrentGameMap2 : ScriptableObject
{
    private Dictionary<string, MapNode> _map;
    public GameMap2 Map { get; private set; }
    public bool IsMapGenerated { get; private set; }
    public List<MapNode> GeneratedMap { get; private set; }
    public string CurrentPositionId { get; private set; }
    public Dictionary<string, MapNodeGameObject> GameObjects { get; set; }
    public int NumMovesMade { get; set; }

    public void SetMap(GameMap2 map)
    {
        Map = map;
        IsMapGenerated = false;
        NumMovesMade = 0;
    }

    public void SetupMap(List<MapNode> generatedMap)
    {
        GeneratedMap = generatedMap;
        IsMapGenerated = true;
        CurrentPositionId = generatedMap.First().NodeId;
        _map = new Dictionary<string, MapNode>();
        GeneratedMap.ForEach(x => _map[x.NodeId] = x);
    }

    public MapNode CurrentMapNode => GetMapNode(CurrentPositionId);
    public MapNodeGameObject CurrentMapNodeGameObject => GameObjects[CurrentPositionId];
    public MapNode GetMapNode(string id) => _map[id];
    public void MoveTo(string id)
    {
        NumMovesMade++;
        CurrentPositionId = id;
    }

    public MapNodeGameObject[] AllGameObjects => GameObjects.Values.ToArray();
}