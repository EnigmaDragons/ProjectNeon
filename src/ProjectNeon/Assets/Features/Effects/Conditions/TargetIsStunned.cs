using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetIsStunned")]
public class TargetIsStunned : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var notStunned = ctx.Target.Members.Where(m => !m.IsStunnedForCard());
        return notStunned.None()
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{notStunned.Names()} are not stunned");
    }
}