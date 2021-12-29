using System;
using System.Linq;
using Features.GameProgression;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Adventure/FixedEncounter")]
public class SpecificEncounterSegment : StageSegment
{
    [SerializeField] private GameObject battlefield;
    [SerializeField] private bool isElite;
    [SerializeField] private bool isTutorial;
    [SerializeField] private Enemy[] enemies;
    [SerializeField] private CurrentAdventureProgress currentAdventureProgress;

    public override string Name => "Specific Encounter";
    public override bool ShouldCountTowardsEnemyPowerLevel => true;
    public override Maybe<string> Detail => Maybe<string>.Missing();
    
    public override void Start()
    {
        Message.Publish(new EnterSpecificBattle(battlefield, isElite, enemies.Select(x => x.ForStage(currentAdventureProgress.AdventureProgress.CurrentChapterNumber)).ToArray(), false, isTutorial));
    }

    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;
}
