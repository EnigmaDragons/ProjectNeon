using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapSpawner3 : OnMessage<NodeFinished>
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
    
    protected override void Execute(NodeFinished msg) => GenerateNewNodes();
    
    private void Awake()
    {
        _activeNodes = new MapNodeGameObject3[0];
        _map = Instantiate(gameMap.CurrentMap.Background, transform);
        _rules = progress.CurrentChapter.NodeTypeOdds2.GenerateMapRules().Concat(new MapGenerationRule3[]
        {
            new EnsureHelpfulOptionsBeforeBoss(),
            new NoClinicsIfYouAreHighHealth(),
            new NoShopsIfYouAreLowOnMoney(),
            new EnsureAtLeastThreeChoices(),
            new OnlyBossOnFinalNode(),
            new EnsureNodeTypeHasCorp(MapNodeType.GearShop, allCorps.GearSellingCorps),
            new EnsureNodeTypeHasCorp(MapNodeType.Clinic, allCorps.ClinicCorps),
        }).ToArray();
        SpawnToken(_map.gameObject);
        StartPlayerTokenFloating();
        if (!gameMap.CurrentChoices.Any())
            GenerateNewNodes();
        else
            SpawnNodes();
        ShowMapPromptIfJustStarted();
    }

    private void ShowMapPromptIfJustStarted()
    {
        if (progress.CurrentStageSegmentIndex == 0)
        {
            var mapPrompt = progress.CurrentAdventure.MapQuestPrompt;
            if (mapPrompt.IsPresent)
                Message.Publish(new ShowInfoDialog(mapPrompt.Value, "Got it!"));
        }
    }

    private void GenerateNewNodes()
    {
        _activeNodes.ForEach(x => Destroy(x.gameObject));
        var nodes = Enum.GetValues(typeof(MapNodeType)).Cast<MapNodeType>().Select(x => new MapNode3 { Type = x }).ToList();
        foreach (var rule in _rules)
            nodes = rule.FilterNodeTypes(nodes, gameMap, partyState, progress);
        var locations = gameMap.CurrentMap.Points.Where(x => x != gameMap.CurrentPosition).ToArray().Shuffled();
        gameMap.CurrentChoices = new List<MapNode3>();
        for (var i = 0; i < nodes.Count; i++)
        {
            nodes[i].Position = locations[i];
            nodes[i].HasEventEnroute = progress.CurrentChapter.NodeTypeOdds2.IsThereTravelEvent(gameMap);
            gameMap.CurrentChoices.Add(nodes[i]);
        }
        SpawnNodes();
    }

    private void SpawnNodes()
    {
        var ctx = new AdventureGenerationContext(progress, allEnemies);
        _activeNodes = gameMap.CurrentChoices.Select(x =>
        {
            var obj = Instantiate(GetNodePrefab(x.Type, x.Corp), _map.transform);
            Action midPoint = x.HasEventEnroute ? () => storyEventSegment.Start() : (Action)(() => travelReactiveSystem.Continue());
            obj.Init(x, ctx, () =>
            {
                gameMap.CompletedNodes.Add(x);
                gameMap.CurrentChoices = new List<MapNode3>();
            }, midPoint);
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
        rect.anchoredPosition = gameMap.CurrentPosition;
        travelReactiveSystem.PlayerToken = _playerToken;
    }

    private void StartPlayerTokenFloating()
    {
        var floating = _playerToken.GetComponent<Floating>();
        if (floating != null)
            floating.enabled = true;
    }
}