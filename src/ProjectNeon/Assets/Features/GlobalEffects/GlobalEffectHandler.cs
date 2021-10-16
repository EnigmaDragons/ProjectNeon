using UnityEngine;

public class GlobalEffectHandler : OnMessage<ApplyGlobalEffect, ClearGlobalEffects>
{
    [SerializeField] private CurrentGlobalEffects effects;

    protected override void Execute(ApplyGlobalEffect msg)
    {
        effects.Apply(msg.Effect);
    }

    protected override void Execute(ClearGlobalEffects msg)
    {
        effects.Clear();
    }
}
