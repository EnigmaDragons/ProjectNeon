using UnityEngine;
using UnityEditor;

public sealed class ShieldFlat : Effect
{
    private int _amount;

    public ShieldFlat(int amount) {
        _amount = amount;
    }

    public void Apply(EffectContext ctx)
    {
        ctx.Target.Members.ForEach(
            member => member.State.AdjustShield(_amount)
        );
    }
}