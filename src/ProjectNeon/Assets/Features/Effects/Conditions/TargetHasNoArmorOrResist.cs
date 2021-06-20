using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetHasNoArmorOrResist")]
public class TargetHasNoArmorOrResist : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var defenseMembers = ctx.Target.Members.Where(m => m.Armor() > 0 || m.Resistance() > 0);
        return defenseMembers.Any()
            ? new Maybe<string>($"{defenseMembers.Names()} have Armor or Resistance")
            : Maybe<string>.Missing();
    }
}
