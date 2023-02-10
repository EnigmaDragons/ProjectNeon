using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "TargetConditions/Or")]
public sealed class OrTargetedCardCondition : StaticTargetedCardCondition
{
    [SerializeField] private StaticTargetedCardCondition[] conditions;

    public override bool ConditionMet(TargetedCardConditionContext ctx) 
        => inversed != conditions.Any(t => t.ConditionMet(ctx));
}