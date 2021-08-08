using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetHasPrimaryResource")]
public class TargetHasPrimaryResource : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Target.Members.All(x => x.PrimaryResourceAmount() > 0)
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.Name} has no Primary Resources");
    }
}