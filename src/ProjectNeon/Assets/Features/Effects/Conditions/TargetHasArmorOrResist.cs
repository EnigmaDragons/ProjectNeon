using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetHasArmorOrResist")]
public class TargetHasArmorOrResist : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var defenselessMembers = ctx.Target.Members.Where(m => m.Armor() <= 0 && m.Resistance() <= 0);
        return defenselessMembers.Any()
            ? new Maybe<string>($"{defenselessMembers.Names()} have no Armor or Resistance")
            : Maybe<string>.Missing();
    }
}