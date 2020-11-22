using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Boss")]
public class BossSegment : StageSegment
{
    public override string Name => "Boss Battle";
    public override void Start() => Message.Publish(new EnterBossBattle());
}