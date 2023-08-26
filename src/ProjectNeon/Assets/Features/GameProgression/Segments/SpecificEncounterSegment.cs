using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/FixedEncounter")]
public class SpecificEncounterSegment : StageSegment
{
    //i know this is definitely getting a little hacky making these properties public for StartTutorialBattleButton
    [SerializeField] public GameObject battlefield;
    [SerializeField] public bool isElite;
    [SerializeField] public Enemy[] enemies;
    [SerializeField] private CurrentAdventureProgress currentAdventureProgress;
    [SerializeField] private MapNodeType mapNodeType;
    [SerializeField] private Cutscene cutscene;
    [SerializeField] private bool shouldAutoStart = false;
    [SerializeField] private Boss finalBoss;

    [Header("Tutorial Settings")]
    [SerializeField] private bool isTutorial;
    [SerializeField] public bool shouldOverrideStartingCards;
    [SerializeField] public int overrideStartingCardsCount;
    [SerializeField] public bool allowBasic = true;
    [SerializeField] public bool allowCycleOrDiscard = true;
    [SerializeField] public CardType[] overrideDeck;

    public GameObject Battlefield => battlefield;
    
    public override string Name => "Specific Encounter";
    public override bool ShouldCountTowardsEnemyPowerLevel => true;
    public override bool ShouldAutoStart => shouldAutoStart;
    public override Maybe<string> Detail => Maybe<string>.Missing();
    public override Maybe<string> Corp => Maybe<string>.Missing();
    public Cutscene Cutscene => cutscene;
    public override MapNodeType MapNodeType => 
        mapNodeType != MapNodeType.Unknown && mapNodeType != MapNodeType.Start
            ? mapNodeType 
            : isElite 
                ? MapNodeType.Elite 
                : MapNodeType.Combat;
    
    public override void Start()
    {
        if (finalBoss != null)
            currentAdventureProgress.AdventureProgress.FinalBoss = finalBoss;
        var stage = currentAdventureProgress.HasActiveAdventure
            ? currentAdventureProgress.AdventureProgress.CurrentChapterNumber
            : 0;
        if (cutscene != null)
            Message.Publish(new SetStartBattleCutsceneRequested(cutscene));
        Message.Publish(new EnterSpecificBattle(battlefield, isElite, enemies.Select(x => x.ForStage(stage)).ToArray(), 
            false, overrideDeck?.Length > 0 ? overrideDeck : null, isTutorial, shouldOverrideStartingCards, overrideStartingCardsCount, allowBasic, allowCycleOrDiscard));
    }

    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;
    public override bool ShouldSpawnThisOnMap(CurrentAdventureProgress p) => true;
}
