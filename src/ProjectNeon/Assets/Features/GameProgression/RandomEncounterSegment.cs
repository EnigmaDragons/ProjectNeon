using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Adventure/RandomEncounter")]
public class RandomEncounterSegment : StageSegment
{
    public override string Name => "Battle";
    public override Maybe<string> Detail => Maybe<string>.Missing();

    [SerializeField] private GameObject[] possibleBattlegrounds;
    [SerializeField] private BattleState battleState;
    [SerializeField] private EncounterBuilder encounterBuilder;
    [SerializeField] private int encounterDifficulty;
    
    public override void Start()
    {
        Log.Info("Setting Up Random Encounter");
        battleState.SetNextBattleground(possibleBattlegrounds.Random());
        battleState.SetNextEncounter(encounterBuilder.Generate(encounterDifficulty));
        SceneManager.LoadScene("BattleSceneV2");
    }
    
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx)
        => new GeneratedBattleStageSegment(Name, possibleBattlegrounds.Random(), false, encounterBuilder.Generate(encounterDifficulty).ToArray());
}
