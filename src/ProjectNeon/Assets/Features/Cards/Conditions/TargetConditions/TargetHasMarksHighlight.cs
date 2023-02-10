using UnityEngine;

[CreateAssetMenu(menuName = "TargetConditions/Marked")]
public class TargetHasMarksHighlight : StaticTargetedCardCondition
{
    [SerializeField] private int marks;
    
    public override bool ConditionMet(TargetedCardConditionContext ctx)
        => inversed != ctx.TargetIs(x => x.State[TemporalStatType.Marked] >= marks);
}