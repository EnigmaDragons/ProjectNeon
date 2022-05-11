using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/FixedEncounter")]
public class SpecificEncounterSegment : StageSegment
{
    [SerializeField] private GameObject battlefield;
    [SerializeField] private bool isElite;
    [SerializeField] private Enemy[] enemies;
    [SerializeField] private CurrentAdventureProgress currentAdventureProgress;
    [SerializeField] private MapNodeType mapNodeType;
    [SerializeField] private Cutscene cutscene;
    [SerializeField] private bool shouldAutoStart = false;

    [Header("Tutorial Settings")]
    [SerializeField] private bool isTutorial;
    [SerializeField] private bool shouldOverrideStartingCards;
    [SerializeField] private int overrideStartingCardsCount;
    [SerializeField] private bool allowBasic = true;
    [SerializeField] private CardType[] overrideDeck;

    public override string Name => "Specific Encounter";
    public override bool ShouldCountTowardsEnemyPowerLevel => true;
    public override bool ShouldAutoStart => shouldAutoStart;
    public override Maybe<string> Detail => Maybe<string>.Missing();
    public override Maybe<string> Corp => Maybe<string>.Missing();
    public override MapNodeType MapNodeType => 
        mapNodeType != MapNodeType.Unknown && mapNodeType != MapNodeType.Start
            ? mapNodeType 
            : isElite 
                ? MapNodeType.Elite 
                : MapNodeType.Combat;
    
    public override void Start()
    {
        var stage = currentAdventureProgress.HasActiveAdventure
            ? currentAdventureProgress.AdventureProgress.CurrentChapterNumber
            : 0;
        if (cutscene != null)
            Message.Publish(new SetStartBattleCutsceneRequested(cutscene));
        Message.Publish(new EnterSpecificBattle(battlefield, isElite, enemies.Select(x => x.ForStage(stage)).ToArray(), 
            false, overrideDeck?.Length > 0 ? overrideDeck : null, isTutorial, shouldOverrideStartingCards, overrideStartingCardsCount, allowBasic));
    }

    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;
    public override bool ShouldSpawnThisOnMap(CurrentAdventureProgress p) => true;
}
