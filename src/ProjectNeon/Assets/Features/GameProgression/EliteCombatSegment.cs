using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/EliteCombat")]
public class EliteCombatSegment : StageSegment
{
    public override string Name => "Elite Combat";
    public override void Start() => Message.Publish(new EnterRandomEliteCombat());
}