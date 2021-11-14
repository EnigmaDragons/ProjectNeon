using System;
using System.Linq;
using Features.GameProgression;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Adventure/FixedEncounter")]
class SpecificEncounterSegment : StageSegment
{
    [SerializeField] private GameObject battlefield;
    [SerializeField] private Enemy[] enemies;
    [SerializeField] private BattleState battle;
    [SerializeField] private string displayName = "Boss Battle";
    [SerializeField] private CurrentAdventureProgress currentAdventureProgress;

    public override string Name => displayName;
    public override Maybe<string> Detail => Maybe<string>.Missing();
    
    public override void Start()
    {
        Log.Info("Setting Up Specific Encounter");
        battle.SetNextBattleground(battlefield);
        battle.SetNextEncounter(enemies.Select(x => x.ForStage(currentAdventureProgress.AdventureProgress.CurrentChapterNumber)));
        SceneManager.LoadScene("BattleSceneV2");
    }

    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;
}
