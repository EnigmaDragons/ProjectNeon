using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "EffectConditions/SourceIsAlone")]
public class SourceIsAlone: StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.BattleMembers.Any(x => x.Value.IsConscious() && x.Value.TeamType == ctx.Source.TeamType && x.Value.Id != ctx.Source.Id)
            ? new Maybe<string>($"{ctx.Source.Name} is not alone")
            : Maybe<string>.Missing();
    }
}