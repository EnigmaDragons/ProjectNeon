using UnityEngine;

public sealed class PlayRawBattleEffect
{
    public string EffectName { get; }
    public Vector3 Target { get; }

    public PlayRawBattleEffect(string effectName, Vector3 target)
    {
        EffectName = effectName;
        Target = target;
    }
    
}
