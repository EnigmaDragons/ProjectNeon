using UnityEngine;

public sealed class PlayRawBattleEffect
{
    public string EffectName { get; }
    public Vector3 Target { get; }
    public string Detail { get; }
    public bool Flip { get; }
    
    public PlayRawBattleEffect(string effectName)
        : this(effectName, Vector3.zero, flip: false) {}
    
    public PlayRawBattleEffect(string effectName, Vector3 target, bool flip = false, string detail = "")
    {
        EffectName = effectName;
        Target = target;
        Detail = detail;
        Flip = flip;
    }
}
