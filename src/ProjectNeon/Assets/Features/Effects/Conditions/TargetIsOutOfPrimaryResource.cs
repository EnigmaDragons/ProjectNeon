using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetIsOutOfPrimaryResource")]
public class TargetIsOutOfPrimaryResource : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Target.Members.All(x => x.PrimaryResourceAmount() <= 0)
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.Name} still has some Primary Resources");
    }
}