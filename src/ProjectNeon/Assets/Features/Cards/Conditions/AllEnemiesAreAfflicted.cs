using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/AllEnemiesAreAfflicted")]
public class AllEnemiesAreAfflicted : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AllEnemies(x => x.State.DamageOverTimes().Length > 0);
}
