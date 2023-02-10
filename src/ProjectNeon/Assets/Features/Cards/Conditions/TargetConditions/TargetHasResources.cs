using UnityEngine;

[CreateAssetMenu(menuName = "TargetConditions/Resources")]
public class TargetHasResources : StaticTargetedCardCondition
{
    [SerializeField] private int resources;
    
    public override bool ConditionMet(TargetedCardConditionContext ctx)
        => inversed != ctx.TargetIs(x => x.State.PrimaryResourceAmount >= resources);
}