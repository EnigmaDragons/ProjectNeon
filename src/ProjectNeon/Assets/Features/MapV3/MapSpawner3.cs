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
    
    //Nodes
    [SerializeField] private MapNodeGameObject3 combatNode;
    [SerializeField] private MapNodeGameObject3 eliteCombatNode;
    [SerializeField] private MapNodeGameObject3 cardShopNode;
    [SerializeField] private MapNodeGameObject3 gearShopNode;
    [SerializeField] private MapNodeGameObject3 storyEventNode;
    [SerializeField] private MapNodeGameObject3 bossNode;
    [SerializeField] private MapNodeGameObject3 clinicNode;

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
            new EnsureAtLeastTwoChoices(),
            new OnlyBossOnFinalNode(),
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
        var nodes = Enum.GetValues(typeof(MapNodeType)).Cast<MapNodeType>().ToList();
        foreach (var rule in _rules)
            nodes = rule.FilterNodeTypes(nodes, gameMap, partyState, progress);
        var locations = gameMap.CurrentMap.Points.Where(x => x != gameMap.CurrentPosition).ToArray().Shuffled();
        gameMap.CurrentChoices = new List<MapNode3>();
        for (var i = 0; i < nodes.Count; i++)
            gameMap.CurrentChoices.Add(new MapNode3 { Type = nodes[i], Position = locations[i], HasEventEnroute = progress.CurrentChapter.NodeTypeOdds2.IsThereTravelEvent(gameMap) });
        SpawnNodes();
    }

    private void SpawnNodes()
    {
        var ctx = new AdventureGenerationContext(progress, allEnemies);
        _activeNodes = gameMap.CurrentChoices.Select(x =>
        {
            var obj = Instantiate(GetNodePrefab(x.Type), _map.transform);
            Action midPoint = x.HasEventEnroute ? () => storyEventSegment.Start() : (Action)(() => travelReactiveSystem.Continue());
            obj.Init(x, ctx, () =>
            {
                gameMap.CompletedNodes.Add(x.Type);
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
    
    private MapNodeGameObject3 GetNodePrefab(MapNodeType type)
    {
        MapNodeGameObject3 nodePrefab = null;
        if (type == MapNodeType.Combat)
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