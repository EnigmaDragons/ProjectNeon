using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/NoEnemiesHaveAnyTemporalStatType")]
public class NoEnemiesHaveAnyTemporalStatType : StaticCardCondition
{
    [SerializeField] private TemporalStatType statType;

    public override bool ConditionMet(CardConditionContext ctx)
        => !ctx.BattleState.EnemyMembers.Any(x => x.State[statType] > 0);
}
