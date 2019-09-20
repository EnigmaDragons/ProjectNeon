using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
class SpecificEncounterSegment : StageSegment
{
    [SerializeField] private Enemy[] enemies;

    public override string Name => "Battle";
    
    public override void Start()
    {
        // @todo #1:30min Setup Custom Encounter, instead of letting Battle Scene randomly generate one
        SceneManager.LoadScene("BattleScene");
    }
}
