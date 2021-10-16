using UnityEngine;

public class GlobalEffectHandler : OnMessage<ApplyGlobalEffect, ClearGlobalEffects>
{
    [SerializeField] private CurrentGlobalEffects effects;

    protected override void Execute(ApplyGlobalEffect msg)
    {
        var ctx = new GlobalEffectContext(effects);
        effects.Apply(msg.Effect, ctx);
    }

    protected override void Execute(ClearGlobalEffects msg)
    {
        effects.Clear();
    }
}
