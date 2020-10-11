using UnityEngine;

public sealed class PlayRawBattleEffect
{
    public string EffectName { get; }
    public Vector3 Target { get; }
    public string Detail { get; }

    public PlayRawBattleEffect(string effectName, Vector3 target, string detail = "")
    {
        EffectName = effectName;
        Target = target;
        Detail = detail;
    }
}
