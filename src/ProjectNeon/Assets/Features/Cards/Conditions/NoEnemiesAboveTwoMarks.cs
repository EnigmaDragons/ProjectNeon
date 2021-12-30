using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/NoEnemiesAboveTwoMarks")]
public class NoEnemiesAboveTwoMarks : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.NoEnemy(x => x.State[TemporalStatType.Marked] > 2);
}