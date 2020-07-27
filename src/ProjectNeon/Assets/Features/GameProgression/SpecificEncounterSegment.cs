using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
class SpecificEncounterSegment : StageSegment
{
    [SerializeField] private GameObject battlefield;
    [SerializeField] private Enemy[] enemies;
    [SerializeField] private BattleState battle;
    [SerializeField] private string displayName = "Boss Battle";

    public override string Name => displayName;
    
    public override void Start()
    {
        Debug.Log("Setting Up Specific Encounter");
        battle.SetNextBattleground(battlefield);
        battle.SetNextEncounter(enemies);
        SceneManager.LoadScene("BattleSceneV2");
    }
}
