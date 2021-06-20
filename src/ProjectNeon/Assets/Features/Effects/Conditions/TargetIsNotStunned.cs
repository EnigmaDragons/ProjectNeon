using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetIsNotStunned")]
public class TargetIsNotStunned : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var stunned = ctx.Target.Members.Where(m => m.IsStunnedForCard());
        return stunned.None()
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{stunned.Names()} are stunned");
    }
}