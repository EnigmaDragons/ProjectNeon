using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetIsDamaged")]
public class TargetIsDamaged : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var notDamagedMembers = ctx.Target.Members.Where(m => m.State.MaxHp() == m.State.Hp());
        return notDamagedMembers.Any()
            ? new Maybe<string>($"{notDamagedMembers.EnglishNames()} not damaged")
            : Maybe<string>.Missing();
    }
}