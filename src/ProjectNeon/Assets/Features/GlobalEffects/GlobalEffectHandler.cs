using UnityEngine;

public class GlobalEffectHandler : OnMessage<ApplyGlobalEffect>
{
    [SerializeField] private CurrentGlobalEffects effects;

    protected override void Execute(ApplyGlobalEffect msg)
    {
        effects.Apply(msg.Effect);
    }
}
