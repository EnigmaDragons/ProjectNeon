using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/EnemiesStunned")]
public class AnyEnemyIsStunned : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AnyEnemy(x => x.IsStunnedForCard());
    
    public override string Description => "Thoughts/Condition009".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition009" };
}