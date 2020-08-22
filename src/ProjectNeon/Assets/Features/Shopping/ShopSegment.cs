using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Adventure/ShopSegment")]
public class ShopSegment : StageSegment
{
    public override string Name => "Shop";

    public override void Start() => SceneManager.LoadScene("ShopScene");
}
