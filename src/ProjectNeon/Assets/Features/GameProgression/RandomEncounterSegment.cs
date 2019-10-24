using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomEncounterSegment : StageSegment
{
    public override string Name => "Battle";
    [SerializeField] private GameObject[] possibleBattlegrounds;
    [SerializeField] private BattleState battleState;
    
    public override void Start()
    {
        battleState.SetNextBattleground(possibleBattlegrounds.Random());
        SceneManager.LoadScene("BattleScene");
    }
}
