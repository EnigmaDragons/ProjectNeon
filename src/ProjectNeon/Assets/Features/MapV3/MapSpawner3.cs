using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapSpawner3 : OnMessage<NodeFinished, GuaranteeStoryEvent>
{
    [SerializeField] private CurrentGameMap3 gameMap;
    [SerializeField] private AdventureProgress2 progress;
    [SerializeField] private TravelReactiveSystem travelReactiveSystem;
    [SerializeField] private GameObject playerToken;
    [SerializeField] private PartyAdventureState partyState;
    [SerializeField] private StageSegment storyEventSegment;
    [SerializeField] private AllEnemies allEnemies;
    [SerializeField] private AllCorps allCorps;
    
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

    private MapNodeGameObject3[] _activeNodes;
    private MapGenerationRule3[] _rules;
    private GameObject _playerToken;
    private GameObject _map;
    
    protected override void Execute(NodeFinished msg) => GenerateNewNodes(false);
    protected override void Execute(GuaranteeStoryEvent msg) => GenerateNewNodes(true);
    
    private void Awake()
    {
        _activeNodes = new MapNodeGameObject3[0];
        _map = Instantiate(gameMap.CurrentMap.Background, transform);
        _rules = progress.CurrentChapter.NodeTypeOdds2.GenerateMapRules().Concat(new MapGenerationRule3[]
        {
            // Node Selection
            new EnsureHelpfulOptionsBeforeBoss(gameMap.MaxNodeCompletionHeat, allCorps.ClinicCorps),
            new NoClinicsIfYouAreHighHealth(),
            new NoShopsIfYouAreLowOnMoney(),
            new EnsureAtLeastThreeChoices(),
            new UseHeatUpEventMapNodeIfTriggered(),
            new OnlyBossOnFinalNode(),
            new EnsureAtLeastOneTravelableNode(),
            
            // Hydration
            new AssignCorpToNodeType(MapNodeType.GearShop, allCorps.GearSellingCorps),
            new AssignCorpToNodeType(MapNodeType.Clinic, allCorps.ClinicCorps, c => partyState.Credits > 30 || !c.Name.Equals("Medigeneix")),
            new PreventTravelToRelevantNodes(progress.GlobalEffects.TravelPreventedCorpNodeTypes)
        }).ToArray();
        SpawnToken(_map.gameObject);
        StartPlayerTokenFloating();
        if (!gameMap.CurrentChoices.Any())
            GenerateNewNodes(false);
        else
            SpawnNodes();
    }

    private void Start()
    {
        if (gameMap.CurrentNode.IsPresent && gameMap.CurrentNode.Value.Type != MapNodeType.Start)
        {
            var activeNode = _activeNodes.First(x => x.MapData.Position.x == gameMap.CurrentNode.Value.Position.x && x.MapData.Position.y == gameMap.CurrentNode.Value.Position.y);
            Message.Publish(new TravelToNode
            {
                OnMidPointArrive = gameMap.CurrentNode.Value.HasEventEnroute 
                    ? StartStoryEvent
                    : (Action<Transform>)(_ => Message.Publish(new ContinueTraveling())),
                OnArrive = _ => activeNode.ArrivalSegment.Start(),
                Position = gameMap.CurrentNode.Value.Position,
                TravelInstantly = false
            });
        }
    }
    
    private void GenerateNewNodes(bool guaranteeEvent)
    {
        gameMap.CompleteCurrentNode();
        _activeNodes.ForEach(x => Destroy(x.gameObject));

        var nodes = Enum.GetValues(typeof(MapNodeType)).Cast<MapNodeType>().Select(x => new MapNode3 { Type = x }).ToList();
        foreach (var rule in _rules)
            nodes = rule.Apply(nodes, gameMap, partyState, progress);
        var locations = gameMap.CurrentMap.Points.Where(x => x != gameMap.DestinationPosition).ToArray().Shuffled();
        gameMap.CurrentChoices = new List<MapNode3>();
        for (var i = 0; i < nodes.Count; i++)
        {
            nodes[i].Position = locations[i];
            nodes[i].HasEventEnroute = guaranteeEvent || progress.CurrentChapter.NodeTypeOdds2.IsThereTravelEvent(gameMap);
            gameMap.CurrentChoices.Add(nodes[i]);
        }
        
        progress.TriggeredHeatUpEvent.IfPresent(e =>
        {
            Message.Publish(new ShowInfoDialog(e.Value.InfoText, "Got it!"));
            progress.RecordFinishedHeatUpEvent(e.Index);
        });
        SpawnNodes();
    }

    private void SpawnNodes()
    {
        var ctx = new AdventureGenerationContext(progress, allEnemies);
        var fx = progress.GlobalEffects.AllStaticGlobalEffects;
        _activeNodes = gameMap.CurrentChoices.Select(x =>
        {
            var obj = Instantiate(GetNodePrefab(x.Type, x.Corp), _map.transform);
            var midPoint = x.HasEventEnroute 
                ? StartStoryEvent
                : (Action<Transform>)(_ => Message.Publish(new ContinueTraveling()));
            obj.Init(x, gameMap, ctx, fx, midPoint);
            var rect = (RectTransform) obj.transform;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = x.Position;
            return obj;
        }).ToArray();
        _playerToken.transform.SetAsLastSibling();
        Message.Publish(new AutoSaveRequested());
    }

    private void StartStoryEvent(Transform player)
    {
        Message.Publish(new TravelMovementStopped(player));
        storyEventSegment.Start();
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