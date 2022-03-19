using System;
using System.Linq;
using UnityEngine;

public class MapSpawner5 : OnMessage<NodeFinished>
{
    [SerializeField] private CurrentGameMap3 gameMap;
    [SerializeField] private AdventureProgressV5 progress;
    [SerializeField] private TravelReactiveSystem travelReactiveSystem;
    [SerializeField] private GameObject playerToken;
    
    //Nodes
    [SerializeField] private MapNodeGameObject3 combatNode;
    [SerializeField] private MapNodeGameObject3 eliteCombatNode;
    [SerializeField] private MapNodeGameObject3 cardShopNode;
    [SerializeField] private MapNodeGameObject3 gearShopNode;
    [SerializeField] private MapNodeGameObject3 storyEventNode;
    [SerializeField] private MapNodeGameObject3 bossNode;
    [SerializeField] private MapNodeGameObject3 clinicNode;
    
    // Corp Nodes
    [SerializeField] private CorpTypedNode[] corpGearNodes;
    [SerializeField] private CorpTypedNode[] corpClinicNodes;

    private MapNodeGameObject3[] _activeNodes = new MapNodeGameObject3[0];

    private GameObject _playerToken;
    private GameObject _map;
    
    protected override void Execute(NodeFinished msg) => SpawnNodes();
    
    private void Awake()
    {
        _map = Instantiate(gameMap.CurrentMap.Background, transform);
        SpawnToken(_map.gameObject);
        StartPlayerTokenFloating();
        SpawnNodes();
    }

    private void Start()
    {
        if (gameMap.CurrentNode.IsPresent && gameMap.CurrentNode.Value.Type != MapNodeType.Start)
        {
            var activeNode = _activeNodes.First(x => x.MapData.Position.x == gameMap.CurrentNode.Value.Position.x && x.MapData.Position.y == gameMap.CurrentNode.Value.Position.y);
            Message.Publish(new TravelToNode
            {
                OnMidPointArrive = (_ => Message.Publish(new ContinueTraveling())),
                OnArrive = _ => activeNode.ArrivalSegment.Start(),
                Position = gameMap.CurrentNode.Value.Position,
                TravelInstantly = false
            });
        }
    }

    private void InitOptions()
    {
        var stageSegments = progress.SecondarySegments.Concat(progress.CurrentStageSegment);
        gameMap.CurrentChoices = stageSegments.Select(s => new MapNode3
        {
            Type = s.MapNodeType,
            Corp = s.Corp.OrDefault(""),
            PresetStage = s
        }).ToList();
    }
    
    private void SpawnNodes()
    {
        InitOptions();
        var fx = progress.GlobalEffects.AllStaticGlobalEffects;
        _activeNodes = gameMap.CurrentChoices.Select(x =>
        {
            var obj = Instantiate(GetNodePrefab(x.Type, x.Corp), _map.transform);
            obj.Init(x, gameMap, x.PresetStage, fx, _ => Message.Publish(new ContinueTraveling()));
            var rect = (RectTransform) obj.transform;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = x.Position;
            return obj;
        }).ToArray();
        _playerToken.transform.SetAsLastSibling();
        Message.Publish(new AutoSaveRequested());
    }

    private MapNodeGameObject3 GetNodePrefab(MapNodeType type, string corpName)
    {
        MapNodeGameObject3 nodePrefab = null;
        if (type == MapNodeType.Combat)
            nodePrefab = combatNode;
        else if (type == MapNodeType.Elite)
            nodePrefab = eliteCombatNode;
        else if (type == MapNodeType.CardShop)
            nodePrefab = cardShopNode;
        else if (type == MapNodeType.GearShop)
            nodePrefab = GetCorpGearShop(corpName);
        else if (type == MapNodeType.StoryEvent)
            nodePrefab = storyEventNode;
        else if (type == MapNodeType.Boss)
            nodePrefab = bossNode;
        else if (type == MapNodeType.Clinic)
            nodePrefab = GetCorpClinicShop(corpName);
        return nodePrefab;
    }

    private MapNodeGameObject3 GetCorpGearShop(string corpName)
        => GetCorpNode(corpName, gearShopNode, corpGearNodes);

    private MapNodeGameObject3 GetCorpClinicShop(string corpName)
        => GetCorpNode(corpName, clinicNode, corpClinicNodes);

    private MapNodeGameObject3 GetCorpNode(string corpName, MapNodeGameObject3 defaultNode, CorpTypedNode[] corpNodes)
    {
        var matchingCorpNodes = !string.IsNullOrWhiteSpace(corpName)
            ? corpNodes.Where(x => x.Corp.Name.Equals(corpName)).ToArray()
            : Array.Empty<CorpTypedNode>();
        if (matchingCorpNodes.Length > 0)
            defaultNode = matchingCorpNodes[0].Object;
        return defaultNode;
    }

    private void SpawnToken(GameObject map)
    {
        progress.InitIfNeeded();
        _playerToken = Instantiate(playerToken, map.transform);
        var rect = (RectTransform)_playerToken.transform;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = gameMap.DestinationPosition;
        travelReactiveSystem.PlayerToken = _playerToken;
    }

    private void StartPlayerTokenFloating()
    {
        var floating = _playerToken.GetComponent<Floating>();
        if (floating != null)
            floating.enabled = true;
    }
}
