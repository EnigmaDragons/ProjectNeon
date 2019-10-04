using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
class ShopSegment : StageSegment
{
    public override string Name => "Shop";

    public override void Start() => SceneManager.LoadScene("GameScene");
}
