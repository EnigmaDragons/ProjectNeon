using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/NoEnemiesHaveAnyTemporalStatType")]
public class NoEnemiesHaveAnyTemporalStatType : StaticCardCondition
{
    [SerializeField] private TemporalStatType statType;

    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.NoEnemy(x => x.State[statType] > 0);
}
