using System.Linq;
using UnityEngine;

public class SwapLifeForce : Effect
{
    public void Apply(EffectContext ctx)
    {
        var originatorLife = ctx.Source.State.Hp();
        var targetLife = ctx.Target.Members.Where(x => x.IsConscious()).Max(x => x.CurrentHp());
        ctx.Source.State.SetHp(Mathf.Min(targetLife, ctx.Source.MaxHp()));
        ctx.Target.ApplyToAllConscious(x => x.SetHp(Mathf.Min(originatorLife, x.MaxHp())));
    }
}