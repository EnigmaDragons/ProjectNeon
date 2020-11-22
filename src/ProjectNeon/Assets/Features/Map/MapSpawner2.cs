using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class MapSpawner2 : MonoBehaviour
{
    [SerializeField] private CurrentGameMap2 gameMap;
    [SerializeField] private AdventureProgress2 progress;
    [SerializeField] private TravelReactiveSystem travelReactiveSystem;
    [SerializeField] private GameObject playerToken;
    [SerializeField] private MapLine line;
    [SerializeField] private GameObject empty;
    
    //Map Inspecific Rules
    [SerializeField] private int nodeVerticalJitter;
    [SerializeField] private int nodeHorizontalJitter;
    
    //Nodes
    [SerializeField] private MapNodeGameObject startNode;
    [SerializeField] private MapNodeGameObject combatNode;
    [SerializeField] private MapNodeGameObject bossNode;

    private Dictionary<string, GameObject> _nodes;
    private GameObject _playerToken;
    
    private void Awake()
    {
        _nodes = new Dictionary<string, GameObject>();
        var map = Instantiate(gameMap.Map.ArtPrototype, transform);
        if (!gameMap.IsMapGenerated)
            GenerateMap();
        var lines = Instantiate(empty, map.transform);
        var nodes = Instantiate(empty, map.transform);
        var mapRect = map.GetComponent<RectTransform>();
        var corners = new Vector3[4];
        mapRect.GetWorldCorners(corners);
        SpawnNodes((RectTransform)nodes.transform, corners[1]);
        SpawnLines((RectTransform)lines.transform);
        SpawnToken(map);
    }
    
    private void Start() => Message.Publish(new FocusOnMapElement { MapElement = (RectTransform)_playerToken.transform });
    
    private void GenerateMap()
    {
        var size = gameMap.Map.ArtPrototype.GetComponent<RectTransform>().sizeDelta;
        var columnSize = (size.x - gameMap.Map.LeftMargin - gameMap.Map.RightMargin) / (progress.CurrentStage.SegmentCount + 1);
        var height = size.y - gameMap.Map.BottomMargin - gameMap.Map.TopMargin;
        var columns = new List<List<MapNode>>
        {
            new List<MapNode> { MapNode.GenerateNew(MapNodeType.Start, 
                x: (int)Mathf.Round(columnSize / 2 + gameMap.Map.LeftMargin), 
                y: (int)Mathf.Round(height / 2 + gameMap.Map.TopMargin)) }, 
            new List<MapNode> { MapNode.GenerateNew(MapNodeType.Boss, 
                x: (int)Mathf.Round(columnSize / 2 + gameMap.Map.LeftMargin + columnSize * progress.CurrentStage.SegmentCount), 
                y: (int)Mathf.Round(height / 2 + gameMap.Map.TopMargin)) }
        };
        for (var column = 1; column < progress.CurrentStage.SegmentCount; column++)
        {
            var nodesInColumn = Rng.Int(gameMap.Map.MinPaths, gameMap.Map.MaxPaths + 1);
            var rowSize = height / nodesInColumn;
            columns.Insert(column, Enumerable.Range(0, nodesInColumn).Select(row => MapNode.GenerateNew(MapNodeType.Combat, 
                x: (int)Mathf.Round(columnSize / 2 + gameMap.Map.LeftMargin + columnSize * column) + Rng.Int(-nodeHorizontalJitter, nodeHorizontalJitter + 1), 
                y: (int)Mathf.Round(rowSize / 2 + gameMap.Map.TopMargin + rowSize * row) + Rng.Int(-nodeVerticalJitter, nodeVerticalJitter + 1))).ToList());
        }
        new ConnectionGenerator().AddConnections(columns);
        gameMap.SetupMap(columns.SelectMany(x => x).OrderBy(x => x.X).ToList());
    }

    private void SpawnNodes(RectTransform map, Vector2 topLeftCorner)
    {
        var playerNode = gameMap.GetMapNode(gameMap.CurrentPositionId);
        var travelIds = playerNode.ChildrenIds;
        foreach (var nodeToSpawn in gameMap.GeneratedMap)
        {
            MapNodeGameObject nodePrefab = null;
            if (nodeToSpawn.Type == MapNodeType.Start)
                nodePrefab = startNode;
            else if (nodeToSpawn.Type == MapNodeType.Combat)
                nodePrefab = combatNode;
            else if (nodeToSpawn.Type == MapNodeType.Boss)
                nodePrefab = bossNode;
            var nodeObject = Instantiate(nodePrefab, new Vector3(topLeftCorner.x + nodeToSpawn.X, topLeftCorner.y - nodeToSpawn.Y, 0), Quaternion.identity, map);
            nodeObject.Init(nodeToSpawn.NodeId, travelIds.Any(x => x == nodeToSpawn.NodeId));
            _nodes[nodeToSpawn.NodeId] = nodeObject.gameObject;
        }
    }

    private void SpawnLines(RectTransform map)
    {
        foreach (var node in gameMap.GeneratedMap)
        {
            foreach (var childId in node.ChildrenIds)
            {
                var lineObj = Instantiate(line, map);
                lineObj.SetPoints((RectTransform)_nodes[node.NodeId].transform, (RectTransform)_nodes[childId].transform);
            }
        }
    }
    
    private void SpawnToken(GameObject map)
    {
        progress.InitIfNeeded();
        _playerToken = Instantiate(playerToken, map.transform);
        var rect = (RectTransform)_playerToken.transform;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = ((RectTransform) _nodes[gameMap.CurrentPositionId].transform).anchoredPosition;
        travelReactiveSystem.PlayerToken = _playerToken;
        var floating = _playerToken.GetComponent<Floating>();
        if (floating != null)
            floating.enabled = true;
    }
}