using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetHasMissingMaxHealth")]
public class TargetHasMissingMaxHealth : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var membersWithoutReducedMaxHealth = ctx.Target.Members.Where(m => m.MaxHp() >= m.State.BaseStats.MaxHp());
        return membersWithoutReducedMaxHealth.Any()
            ? new Maybe<string>($"{membersWithoutReducedMaxHealth.Names()} are not missing max health")
            : Maybe<string>.Missing();
    }
}