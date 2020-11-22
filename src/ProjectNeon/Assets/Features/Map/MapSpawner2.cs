using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class MapSpawner2 : MonoBehaviour
{
    [SerializeField] private CurrentGameMap2 gameMap;
    [SerializeField] private AdventureProgress progress;

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

    private void Awake()
    {
        var map = Instantiate(gameMap.Map.ArtPrototype, transform);
        if (!gameMap.IsMapGenerated)
            GenerateMap();
        SpawnNode(map, gameMap.GeneratedMap, false);
    }
    
    private void GenerateMap()
    {
        var bounds = gameMap.Map.ArtPrototype.GetComponentInChildren<Renderer>().bounds;
        var columnSize = (bounds.size.x - leftMargin - rightMargin) / (gameMap.Map.PathLength + 1);
        var height = bounds.size.y - bottomMargin - topMargin;
        var columns = new List<List<MapNode>>
        {
            new List<MapNode> { MapNode.GenerateNew(MapNodeType.Start, 
                x: (int)Mathf.Round(columnSize / 2 + leftMargin), 
                y: (int)Mathf.Round(height / 2 + topMargin)) }, 
            new List<MapNode> { MapNode.GenerateNew(MapNodeType.Boss, 
                x: (int)Mathf.Round(columnSize / 2 + leftMargin + columnSize * gameMap.Map.PathLength), 
                y: (int)Mathf.Round(height / 2 + topMargin)) }
        };
        for (var column = 1; column < gameMap.Map.PathLength; column++)
        {
            var nodesInColumn = Rng.Int(gameMap.Map.MinPaths, gameMap.Map.MaxPaths + 1);
            var rowSize = height / nodesInColumn;
            columns.Insert(0, Enumerable.Range(0, nodesInColumn).Select(row => MapNode.GenerateNew(MapNodeType.Combat, 
                x: (int)Mathf.Round(columnSize / 2 + leftMargin + columnSize * column) + Rng.Int(-nodeHorizontalJitter, nodeHorizontalJitter + 1), 
                y: (int)Mathf.Round(rowSize / 2 + topMargin + rowSize * row) + Rng.Int(-nodeVerticalJitter, nodeVerticalJitter + 1))).ToList());
        }
        new ConnectionGenerator().AddConnections(columns);
        gameMap.SetupMap(columns[0][0]);
    }

    private void SpawnNode(GameObject map, MapNode node, bool canTravelTo)
    {
        if (node.Type == MapNodeType.Start)
            Instantiate(startNode, new Vector3(node.X, node.Y, 0), Quaternion.identity, map.transform).Init(canTravelTo);
        else if (node.Type == MapNodeType.Combat)
            Instantiate(combatNode, new Vector3(node.X, node.Y, 0), Quaternion.identity, map.transform).Init(canTravelTo);
        else if (node.Type == MapNodeType.Boss)
            Instantiate(bossNode, new Vector3(node.X, node.Y, 0), Quaternion.identity, map.transform).Init(canTravelTo);
        node.Children.ForEach(x => SpawnNode(map, x, node.NodeId == gameMap.CurrentPositionId));
    }
    
    /*private void SpawnToken()
    {
        progress.InitIfNeeded();
        var o = Instantiate(tokenPrototype, transform);
        var rectTransform = o.GetComponent<RectTransform>();
        rectTransform.anchoredPosition += map.Locations[progress.CurrentStageSegmentIndex].GeoPosition;
        var floating = o.GetComponent<Floating>();
        if (floating != null)
            floating.enabled = true;
    }*/
}