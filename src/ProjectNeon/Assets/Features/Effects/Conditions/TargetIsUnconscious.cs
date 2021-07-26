using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetIsUnconscious")]
public class TargetIsUnconscious : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var conscious = ctx.Target.Members.Where(m => m.IsConscious());
        return conscious.None()
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{conscious.Names()} are not stunned");
    }
}