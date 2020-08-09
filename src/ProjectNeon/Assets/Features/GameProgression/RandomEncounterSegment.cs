using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class RandomEncounterSegment : StageSegment
{
    public override string Name => "Battle";
    
    [SerializeField] private GameObject[] possibleBattlegrounds;
    [SerializeField] private BattleState battleState;
    [SerializeField] private EncounterBuilder encounterBuilder;
    [SerializeField] private int encounterDifficulty;
    
    public override void Start()
    {
        Debug.Log("Setting Up Random Encounter");
        battleState.SetNextBattleground(possibleBattlegrounds.Random());
        battleState.SetNextEncounter(encounterBuilder.Generate(encounterDifficulty));
        SceneManager.LoadScene("BattleSceneV2");
    }
}
