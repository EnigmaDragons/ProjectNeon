using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapSpawner2 : MonoBehaviour
{
    [SerializeField] private CurrentGameMap2 gameMap;
    [SerializeField] private AdventureProgress2 progress;
    [SerializeField] private TravelReactiveSystem travelReactiveSystem;
    [SerializeField] private GameObject playerToken;
    [SerializeField] private MapLine line;
    [SerializeField] private GameObject empty;
    [SerializeField] private AllEnemies enemies;

    //Map Inspecific Rules
    [SerializeField] private int nodeVerticalJitter;
    [SerializeField] private int nodeHorizontalJitter;
    
    //Nodes
    [SerializeField] private MapNodeGameObject startNode;
    [SerializeField] private MapNodeGameObject combatNode;
    [SerializeField] private MapNodeGameObject eliteCombatNode;
    [SerializeField] private MapNodeGameObject cardShopNode;
    [SerializeField] private MapNodeGameObject gearShopNode;
    [SerializeField] private MapNodeGameObject storyEventNode;
    [SerializeField] private MapNodeGameObject bossNode;
    [SerializeField] private MapNodeGameObject clinicNode;
    
    // UI Configuration
    [SerializeField] private Vector4 marginRightBottomLeftTop;
    [SerializeField] private MovableMap.MovableMapSettings mapMovementSettings;

    private Vector4 Margin => marginRightBottomLeftTop;
    private GameObject _playerToken;
    private NodeTypeAssigner _assigner;
    
    private void Awake()
    {
        _assigner = new NodeTypeAssigner(progress);
        gameMap.GameObjects = new Dictionary<string, MapNodeGameObject>();
        var map = Instantiate(gameMap.Map.ArtPrototype, transform).Initialized(mapMovementSettings);
        if (!gameMap.IsMapGenerated)
            GenerateMap();
        var lines = Instantiate(empty, map.transform);
        var nodes = Instantiate(empty, map.transform);
        var mapRect = map.GetComponent<RectTransform>();
        var corners = new Vector3[4];
        mapRect.GetWorldCorners(corners);
        SpawnNodes((RectTransform)nodes.transform, corners[1]);
        SpawnLines((RectTransform)lines.transform);
        ConvertAdjacentNodesToDeterministic();
        SpawnToken(map.gameObject);
        TravelNextIfNeededBasedOnAdventureProgress();
        StartPlayerTokenFloating();
    }

    private void TravelNextIfNeededBasedOnAdventureProgress()
    {
        //OBSOLETED
        var numToMove = 5 - gameMap.NumMovesMade;
        Log.Info($"{numToMove} remaining spaces to move.");
        if (numToMove == 0)
            return;
        
        var currentNode = gameMap.CurrentMapNode;
        var destinationNodeId = currentNode.ChildrenIds.Random();
        var destinationNodeGameObject = gameMap.GameObjects[destinationNodeId];
        Message.Publish(new TravelToNode { 
            OnArrive = () =>
            {
                Log.Info("Arrived at Location");
                ConvertAdjacentNodesToDeterministic();
                TravelNextIfNeededBasedOnAdventureProgress();
            },
            Node = destinationNodeGameObject.gameObject,
            NodeId = destinationNodeId,
            TravelInstantly = true
        });
    }
    
    private void Start() => Message.Publish(new FocusOnMapElement { MapElement = (RectTransform)_playerToken.transform });
    
    private void GenerateMap()
    {
        var size = gameMap.Map.ArtPrototype.GetComponent<RectTransform>().sizeDelta;
        var columnSize = (size.x - Margin.z - Margin.x) / (progress.CurrentChapter.SegmentCount + 1);
        var height = size.y - Margin.y - Margin.w;
        var startNode = MapNode.GenerateNew(
            x: (int) Mathf.Round(columnSize / 2 + Margin.z),
            y: (int) Mathf.Round(height / 2 + Margin.w));
        startNode.Type = MapNodeType.Start;
        var bossNode = MapNode.GenerateNew(
            x: (int) Mathf.Round(columnSize / 2 + Margin.z + columnSize * progress.CurrentChapter.SegmentCount),
            y: (int) Mathf.Round(height / 2 + Margin.w));
        bossNode.Type = MapNodeType.Boss;
        var columns = new List<List<MapNode>> { new List<MapNode> { startNode }, new List<MapNode> { bossNode } };
        for (var column = 1; column < progress.CurrentChapter.SegmentCount; column++)
        {
            var nodesInColumn = Rng.Int(gameMap.Map.MinPaths, gameMap.Map.MaxPaths + 1);
            var rowSize = height / nodesInColumn;
            columns.Insert(column, Enumerable.Range(0, nodesInColumn)
                .Select(row => MapNode.GenerateNew(ColumnX(columnSize, column), RowY(rowSize, row)))
                .ToList());
        }
        new ConnectionGenerator().AddConnections(columns);
        _assigner.Assign(columns);
        gameMap.SetupMap(columns.SelectMany(x => x).OrderBy(x => x.X).ToList());
    }

    private void ConvertAdjacentNodesToDeterministic()
    {
        foreach (var childId in gameMap.GetMapNode(gameMap.CurrentPositionId).ChildrenIds)
            gameMap.GameObjects[childId].ConvertToDeterministic(new AdventureGenerationContext(progress, enemies));
    }

    private int ColumnX(float columnSize, int column) 
        => (int)Mathf.Round(columnSize / 2 + Margin.z + columnSize * column) + Rng.Int(-nodeHorizontalJitter, nodeHorizontalJitter + 1);

    private int RowY(float rowSize, int row)
        => (int)Mathf.Round(rowSize / 2 + Margin.w + rowSize * row) + Rng.Int(-nodeVerticalJitter, nodeVerticalJitter + 1);

    private void SpawnNodes(RectTransform map, Vector2 topLeftCorner)
    {
        var playerNode = gameMap.GetMapNode(gameMap.CurrentPositionId);
        var travelIds = playerNode.ChildrenIds;
        foreach (var nodeToSpawn in gameMap.GeneratedMap)
        {
            var nodePrefab = GetNodePrefab(nodeToSpawn.Type);
            var nodeObject = Instantiate(nodePrefab, new Vector3(topLeftCorner.x + nodeToSpawn.X, topLeftCorner.y - nodeToSpawn.Y, 0), Quaternion.identity, map);
            var canTravelTo = travelIds.Any(x => x == nodeToSpawn.NodeId);
            nodeObject.Init(nodeToSpawn.NodeId, canTravelTo);
            gameMap.GameObjects[nodeToSpawn.NodeId] = nodeObject;
        }
    }

    private MapNodeGameObject GetNodePrefab(MapNodeType type)
    {
        MapNodeGameObject nodePrefab = null;
        if (type == MapNodeType.Start)
            nodePrefab = startNode;
        else if (type == MapNodeType.Combat)
            nodePrefab = combatNode;
        else if (type == MapNodeType.Elite)
            nodePrefab = eliteCombatNode;
        else if (type == MapNodeType.CardShop)
            nodePrefab = cardShopNode;
        else if (type == MapNodeType.GearShop)
            nodePrefab = gearShopNode;
        else if (type == MapNodeType.StoryEvent)
            nodePrefab = storyEventNode;
        else if (type == MapNodeType.Boss)
            nodePrefab = bossNode;
        else if (type == MapNodeType.Clinic)
            nodePrefab = clinicNode;
        return nodePrefab;
    }

    private void SpawnLines(RectTransform map)
    {
        foreach (var node in gameMap.GeneratedMap)
        {
            foreach (var childId in node.ChildrenIds)
            {
                var lineObj = Instantiate(line, map);
                lineObj.SetPoints((RectTransform)gameMap.GameObjects[node.NodeId].transform, (RectTransform)gameMap.GameObjects[childId].transform);
            }
        }
    }
    
    private void SpawnToken(GameObject map)
    {
        progress.InitIfNeeded();
        _playerToken = Instantiate(playerToken, map.transform);
        var rect = (RectTransform)_playerToken.transform;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = ((RectTransform) gameMap.GameObjects[gameMap.CurrentPositionId].transform).anchoredPosition;
        travelReactiveSystem.PlayerToken = _playerToken;
    }

    private void StartPlayerTokenFloating()
    {
        var floating = _playerToken.GetComponent<Floating>();
        if (floating != null)
            floating.enabled = true;
    }
}