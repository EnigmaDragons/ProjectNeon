using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/StaticNotCondition")]
public class StaticNotCondition : StaticCardCondition
{
    [SerializeField] private StaticCardCondition baseCondition;

    public override bool ConditionMet(CardConditionContext ctx) => !baseCondition.ConditionMet(ctx);
}
