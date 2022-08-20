using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class MapSpawner5 : OnMessage<RegenerateMapRequested, SkipSegment>
{
    [SerializeField] private CurrentMapSegmentV5 gameMap;
    [SerializeField] private CurrentAdventureProgress currentProgress;
    [SerializeField] private AdventureProgressV5 progress;
    [SerializeField] private TravelReactiveSystem travelReactiveSystem;
    [SerializeField] private GameObject playerToken;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private StageSegment shopSegment;
    [SerializeField] private UILineRenderer pathLinePrototype;
    [SerializeField] private GameObject pathLinesParent;
    [SerializeField] private GameObject mapNodesParent;
    [SerializeField] private GameObject playerTokenParent;
    [SerializeField] private AllStageSegments allStageSegments;
    [SerializeField] private FloatReference minDistanceBetweenNodes = new FloatReference(0f);
    [SerializeField] private CurrentTheme currentTheme;

    //Nodes
    [SerializeField] private MapNodeGameObject3 combatNode;
    [SerializeField] private MapNodeGameObject3 eliteCombatNode;
    [SerializeField] private MapNodeGameObject3 cardShopNode;
    [SerializeField] private MapNodeGameObject3 gearShopNode;
    [SerializeField] private MapNodeGameObject3 storyEventNode;
    [SerializeField] private MapNodeGameObject3 bossNode;
    [SerializeField] private MapNodeGameObject3 clinicNode;
    [SerializeField] private MapNodeGameObject3 mainStoryNode;
    
    // Corp Nodes
    [SerializeField] private CorpTypedNode[] corpGearNodes;
    [SerializeField] private CorpTypedNode[] corpClinicNodes;

    private GameObject _playerToken;
    private GameObject _map;
    
    protected override void Execute(RegenerateMapRequested msg)
    {
        SpawnPartyToken();
        SpawnNodes();
    }
    
    protected override void Execute(SkipSegment msg)
    {
        progress.Advance();
        gameMap.ClearSegment();
        SpawnPartyToken();
        SpawnNodes();
    }

    private void Awake()
    {
        gameMap.CurrentMap = progress.CurrentChapter.Map;
        if (gameMap.CurrentMap.Theme != null && currentTheme != null)
            currentTheme.Set(gameMap.CurrentMap.Theme);
        _map = Instantiate(gameMap.CurrentMap.Background, transform);
        _map.transform.SetAsFirstSibling();
    }

    private void SpawnNodes()
    {
        if (pathLinesParent != null)
            pathLinesParent.DestroyAllChildren();
        mapNodesParent.DestroyAllChildren();
        
        if (gameMap.CurrentChoices.None(c => c.AdvancesAdventure))
            GenerateOptions();
        var fx = progress.GlobalEffects.AllStaticGlobalEffects;
        Log.Info($"Map Spawning Nodes: {gameMap.CurrentChoices.Count} Nodes. Adventure Progress: Chapter {progress.CurrentChapterNumber}, Segment {progress.CurrentStageProgress}");
        gameMap.CurrentChoices.ForEach(x =>
        {
            Log.Info($"Spawning Node: {x.Type} - {x.Corp}");
            try
            {
                var obj = Instantiate(GetNodePrefab(x.Type, x.Corp), mapNodesParent.transform);
                obj.InitForV5(x, gameMap, allStageSegments.GetStageSegmentById(x.PresetStageId).Value, fx, x.AdvancesAdventure, _ => Message.Publish(new ContinueTraveling()));
                var rect = (RectTransform) obj.transform;
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = x.Position;
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Error($"Node Prefab for {x.Type} - {x.Corp} is null");
            }
        });

        SpawnPathLines();

        if (_playerToken != null)
            _playerToken.transform.SetAsLastSibling();
        Message.Publish(new AutoSaveRequested());
    }

    private void SpawnPathLines()
    {
        if (pathLinesParent == null)
            return;
        
        var allChoices = gameMap.CurrentChoices.OrderBy(c => c.Position.x).ToArray();
        var mapLineCombinations = new List<(Vector2, Vector2)>();
        allChoices.ForEach(c => mapLineCombinations.Add((gameMap.DestinationPosition, c.Position)));
        
        for (var i = 0; i < allChoices.Length; i++)
        {
            var rootChoice = allChoices[i];
            allChoices.Skip(i + 1).ForEach(c => mapLineCombinations.Add((rootChoice.Position, c.Position)));
        }

        mapLineCombinations.ForEach(c =>
        {
            var pathLine = Instantiate(pathLinePrototype, pathLinesParent.transform);
            var leftPoint = c.Item1.x <= c.Item2.x ? c.Item1 : c.Item2;
            var rightPoint = c.Item1.x > c.Item2.x ? c.Item1 : c.Item2;
            pathLine.m_points = new[] {leftPoint, rightPoint};
        });
    }

    private void GenerateOptions()
    {
        var sideSegments = progress.SecondarySegments.ToList();
        var shouldHaveShop = progress.CurrentStageProgress >= progress.CurrentChapter.NoShopUntilSegment 
                             && party.Credits >= 150
                             && progress.CurrentChapter.ShopOdds < Rng.Float();
        if (shouldHaveShop)
            sideSegments.Add(shopSegment);
        
        var stageSegments = sideSegments.Concat(progress.CurrentStageSegment);
        gameMap.CurrentChoices = stageSegments
            .Where(s => s.MapNodeType != MapNodeType.Unknown)
            .Where(s => s.ShouldSpawnThisOnMap(currentProgress))
            .Select(s =>
            {
                var shouldAdvanceAdventure = !sideSegments.Contains(s);
                Log.Info($"Map Node: {s.MapNodeType}. Advances Adventure: {shouldAdvanceAdventure}. {progress}");
                return new MapNode3
                {
                    Type = s.MapNodeType,
                    Corp = s.Corp.OrDefault(""),
                    PresetStage = s,
                    PresetStageId = s.Id,
                    AdvancesAdventure = shouldAdvanceAdventure
                };
            }).ToList();
        
        var locations = gameMap.CurrentMap.Points.Where(x => x != gameMap.DestinationPosition)
            .ToArray()
            .Shuffled(new DeterministicRng(progress.RngSeed))
            .ToList();
        foreach (var choice in gameMap.CurrentChoices)
        {
            if (gameMap.CurrentChoices.Last() == choice)
            {
                choice.Position = gameMap.CurrentMap.EndingPoint;
            }
            else
            {
                if (locations.Count < 1)
                {
                    Log.Error("Not Enough Map Points or Minimum Distance is too high");
                    return;
                }
                var pos = locations[0];
                choice.Position = pos;
                locations.RemoveAll(l => Vector2.Distance(l, pos) < minDistanceBetweenNodes);   
            }
        }
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
        else if (type == MapNodeType.MainStory)
            nodePrefab = mainStoryNode;
        
        if (nodePrefab == null)
            Log.Error($"{nameof(MapSpawner5)} - Unknown Map Node Type {type}");
        
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

    private void SpawnPartyToken()
    {
        if (_playerToken != null)
        {
            Destroy(_playerToken);
            _playerToken = null;
        }
        SpawnPartyTokenIfNeeded(_map.gameObject);
    }

    private void SpawnPartyTokenIfNeeded(GameObject map)
    {
        if (party.Heroes.Length < 1 || _playerToken != null)
            return;
        
        progress.InitIfNeeded();
        _playerToken = Instantiate(playerToken, playerTokenParent.transform);
        var rect = (RectTransform)_playerToken.transform;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = gameMap.DestinationPosition;
        travelReactiveSystem.PlayerToken = _playerToken;
        StartPlayerTokenFloating();
    }

    private void StartPlayerTokenFloating()
    {
        var floating = _playerToken.GetComponent<Floating>();
        if (floating != null)
            floating.enabled = true;
    }
}
