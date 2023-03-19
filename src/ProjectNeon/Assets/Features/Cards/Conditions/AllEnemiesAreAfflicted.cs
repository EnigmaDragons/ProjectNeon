using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/AllEnemiesAreAfflicted")]
public class AllEnemiesAreAfflicted : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AllEnemies(x => x.State.DamageOverTimes().Length > 0);

    public override string Description => "Thoughts/Condition001".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition001" };
}
