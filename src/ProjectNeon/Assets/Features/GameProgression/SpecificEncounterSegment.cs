using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Adventure/FixedEncounter")]
class SpecificEncounterSegment : StageSegment
{
    [SerializeField] private GameObject battlefield;
    [SerializeField] private Enemy[] enemies;
    [SerializeField] private BattleState battle;
    [SerializeField] private string displayName = "Boss Battle";
    [SerializeField] private AdventureProgress2 currentAdventureProgress; 
    
    public override string Name => displayName;
    public override Maybe<string> Detail => Maybe<string>.Missing();
    
    public override void Start()
    {
        Log.Info("Setting Up Specific Encounter");
        battle.SetNextBattleground(battlefield);
        battle.SetNextEncounter(enemies.Select(x => x.GetEnemy(currentAdventureProgress.Stage)));
        SceneManager.LoadScene("BattleSceneV2");
    }

    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx) => this;
}
