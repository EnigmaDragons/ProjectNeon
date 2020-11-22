using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class MapSpawner2 : MonoBehaviour
{
    [SerializeField] private CurrentGameMap2 gameMap;
    [SerializeField] private AdventureProgress progress;
    [SerializeField] private TravelReactiveSystem travelReactiveSystem;
    [SerializeField] private GameObject playerToken;
    
    //Map Inspecific Rules
    [SerializeField] private float bottomMargin;
    [SerializeField] private float leftMargin;
    [SerializeField] private float rightMargin;
    [SerializeField] private float topMargin;
    [SerializeField] private int nodeVerticalJitter;
    [SerializeField] private int nodeHorizontalJitter;
    
    //Nodes
    [SerializeField] private MapNodeGameObject startNode;
    [SerializeField] private MapNodeGameObject combatNode;
    [SerializeField] private MapNodeGameObject bossNode;

    private GameObject _playerNode;
    private GameObject _playerToken;
    
    private void Awake()
    {
        var map = Instantiate(gameMap.Map.ArtPrototype, transform);
        if (!gameMap.IsMapGenerated)
            GenerateMap();
        var mapRect = map.GetComponent<RectTransform>();
        var corners = new Vector3[4];
        mapRect.GetWorldCorners(corners);
        SpawnNode(mapRect, corners[1], gameMap.GeneratedMap.First(), false);
        SpawnToken(map);
    }
    
    private void Start() => Message.Publish(new FocusOnMapElement { MapElement = (RectTransform)_playerToken.transform });
    
    private void GenerateMap()
    {
        var size = gameMap.Map.ArtPrototype.GetComponent<RectTransform>().sizeDelta;
        var columnSize = (size.x - leftMargin - rightMargin) / (progress.CurrentStage.SegmentCount + 1);
        var height = size.y - bottomMargin - topMargin;
        var columns = new List<List<MapNode>>
        {
            new List<MapNode> { MapNode.GenerateNew(MapNodeType.Start, 
                x: (int)Mathf.Round(columnSize / 2 + leftMargin), 
                y: (int)Mathf.Round(height / 2 + topMargin)) }, 
            new List<MapNode> { MapNode.GenerateNew(MapNodeType.Boss, 
                x: (int)Mathf.Round(columnSize / 2 + leftMargin + columnSize * progress.CurrentStage.SegmentCount), 
                y: (int)Mathf.Round(height / 2 + topMargin)) }
        };
        for (var column = 1; column < progress.CurrentStage.SegmentCount; column++)
        {
            var nodesInColumn = Rng.Int(gameMap.Map.MinPaths, gameMap.Map.MaxPaths + 1);
            var rowSize = height / nodesInColumn;
            columns.Insert(column, Enumerable.Range(0, nodesInColumn).Select(row => MapNode.GenerateNew(MapNodeType.Combat, 
                x: (int)Mathf.Round(columnSize / 2 + leftMargin + columnSize * column) + Rng.Int(-nodeHorizontalJitter, nodeHorizontalJitter + 1), 
                y: (int)Mathf.Round(rowSize / 2 + topMargin + rowSize * row) + Rng.Int(-nodeVerticalJitter, nodeVerticalJitter + 1))).ToList());
        }
        new ConnectionGenerator().AddConnections(columns);
        gameMap.SetupMap(columns.SelectMany(x => x).OrderBy(x => x.X).ToList());
    }

    private void SpawnNodes(RectTransform map, Vector2 topLeftCorner, MapNode node, bool canTravelTo)
    {
        var playerNodeId = gameMap.CurrentPositionId
        
            
        var p
        var isPlayerNode = node.NodeId == gameMap.CurrentPositionId;
        MapNodeGameObject nodePrefab = null;
        if (node.Type == MapNodeType.Start)
            nodePrefab = startNode;
        else if (node.Type == MapNodeType.Combat)
            nodePrefab = combatNode;
        else if (node.Type == MapNodeType.Boss)
            nodePrefab = bossNode;
        var nodeObject = Instantiate(nodePrefab, new Vector3(topLeftCorner.x + node.X, topLeftCorner.y - node.Y, 0), Quaternion.identity, map);
        nodeObject.Init(node.NodeId, canTravelTo);
        if (isPlayerNode)
            _playerNode = nodeObject.gameObject;
        foreach (var child in node.ChildrenIds.Select(x => gameMap.GetMapNode(x)))
            SpawnNode(map, topLeftCorner, child, isPlayerNode);
    }
    
    private void SpawnToken(GameObject map)
    {
        progress.InitIfNeeded();
        _playerToken = Instantiate(playerToken, _playerNode.transform.position, Quaternion.identity, map.transform);
        travelReactiveSystem.PlayerToken = _playerToken;
        var floating = _playerToken.GetComponent<Floating>();
        if (floating != null)
            floating.enabled = true;
    }
}