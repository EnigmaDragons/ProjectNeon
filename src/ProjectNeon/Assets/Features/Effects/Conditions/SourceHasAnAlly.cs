using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "EffectConditions/SourceHasAnAlly")]
public class SourceHasAnAlly : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.BattleMembers.Any(x => x.Value.IsConscious() && x.Value.TeamType == ctx.Source.TeamType && x.Value.Id != ctx.Source.Id)
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.Name} has no ally");
    }
}