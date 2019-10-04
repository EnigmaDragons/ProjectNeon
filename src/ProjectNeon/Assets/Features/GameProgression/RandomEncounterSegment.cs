using UnityEngine;
using UnityEngine.SceneManagement;

class RandomEncounterSegment : StageSegment
{
    public override string Name => "Battle";

    public override void Start()
    {
        SceneManager.LoadScene("GameScene");
    }
}
