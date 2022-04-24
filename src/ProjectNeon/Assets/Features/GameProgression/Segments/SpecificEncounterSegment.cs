using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/FixedEncounter")]
public class SpecificEncounterSegment : StageSegment
{
    [SerializeField] private GameObject battlefield;
    [SerializeField] private bool isElite;
    [SerializeField] private bool isTutorial;
    [SerializeField] private Enemy[] enemies;
    [SerializeField] private CurrentAdventureProgress currentAdventureProgress;
    [SerializeField] private MapNodeType mapNodeType;

    public override string Name => "Specific Encounter";
    public override bool ShouldCountTowardsEnemyPowerLevel => true;
    public override bool ShouldAutoStart => false;
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
        Message.Publish(new EnterSpecificBattle(battlefield, isElite, enemies.Select(x => x.ForStage(stage)).ToArray(), false, isTutorial));
    }

    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;
}
