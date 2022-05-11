using System;
using System.Linq;
using UnityEngine;

public class MapSpawner5 : OnMessage<RegenerateMapRequested, SkipSegment>
{
    [SerializeField] private CurrentMapSegmentV5 gameMap;
    [SerializeField] private CurrentAdventureProgress currentProgress;
    [SerializeField] private AdventureProgressV5 progress;
    [SerializeField] private TravelReactiveSystem travelReactiveSystem;
    [SerializeField] private GameObject playerToken;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private StageSegment shopSegment;
    [SerializeField] private GameObject mapNodesParent;
    [SerializeField] private GameObject playerTokenParent;
    [SerializeField] private AllStageSegments allStageSegments;
    [SerializeField] private FloatReference minDistanceBetweenNodes = new FloatReference(0f);
    
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
        SpawnPartyTokenIfNeeded(_map.gameObject);
        SpawnNodes();
    }
    
    protected override void Execute(SkipSegment msg)
    {
        progress.Advance();
        gameMap.ClearSegment();
        SpawnPartyTokenIfNeeded(_map.gameObject);
        SpawnNodes();
    }

    private void Awake()
    {
        if (gameMap.CurrentMap == null)
            gameMap.CurrentMap = progress.CurrentChapter.Map;
        _map = Instantiate(gameMap.CurrentMap.Background, transform);
        _map.transform.SetAsFirstSibling();
    }

    private void SpawnNodes()
    {
        mapNodesParent.DestroyAllChildren();
        
        if (gameMap.CurrentChoices.None())
            GenerateOptions();
        var fx = progress.GlobalEffects.AllStaticGlobalEffects;
        Log.Info($"Map Spawning Nodes: {gameMap.CurrentChoices.Count} Nodes. Adventure Progress: Chapter {progress.CurrentChapterNumber}, Segment {progress.CurrentStageProgress}");
        gameMap.CurrentChoices.ForEach(x =>
        {
            Log.Info($"Spawning Node: {x.Type} - {x.Corp}");
            try
            {
                var obj = Instantiate(GetNodePrefab(x.Type, x.Corp), mapNodesParent.transform);
                obj.InitForV5(x, gameMap, allStageSegments.GetStageSegmentById(x.PresetStageId).Value, fx, _ => Message.Publish(new ContinueTraveling()));
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
        if (_playerToken != null)
            _playerToken.transform.SetAsLastSibling();
        Message.Publish(new AutoSaveRequested());
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
            .Shuffled()
            .ToList();
        foreach (var choice in gameMap.CurrentChoices)
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
