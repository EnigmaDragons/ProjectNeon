using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetIsEnemy")]
public class TargetIsEnemy : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var notEnemyMembers = ctx.Target.Members.Where(m => m.TeamType == ctx.Source.TeamType);
        return notEnemyMembers.Any()
            ? new Maybe<string>($"{notEnemyMembers.Names()} not Enemies of {ctx.Source.Name}")
            : Maybe<string>.Missing();
    }
}
