﻿using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Adventure/RandomEncounter")]
public class RandomEncounterSegment : StageSegment
{
    public override string Name => "Battle";
    public override bool ShouldCountTowardsEnemyPowerLevel => true;
    public override bool ShouldAutoStart => false;
    public override Maybe<string> Detail => Maybe<string>.Missing();
    public override MapNodeType MapNodeType => MapNodeType.Combat;
    public override Maybe<string> Corp => Maybe<string>.Missing();

    [SerializeField] private GameObject[] possibleBattlegrounds;
    [SerializeField] private BattleState battleState;
    [SerializeField] private EncounterBuilder encounterBuilder;
    [SerializeField] private int encounterDifficulty;
    
    public override void Start()
    {
        Log.Info("Setting Up Random Encounter");
        battleState.SetNextBattleground(possibleBattlegrounds.Random());
        battleState.SetNextEncounter(encounterBuilder.Generate(encounterDifficulty, 1));
        SceneManager.LoadScene("BattleSceneV2");
    }
    
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx , MapNode3 mapData)
        => new GeneratedBattleStageSegment(Name, possibleBattlegrounds.Random(), false, encounterBuilder.Generate(encounterDifficulty, 1).ToArray());
}
