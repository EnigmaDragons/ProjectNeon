using System;

[Serializable]
public sealed class EffectData
{
    public bool ShouldApply => EffectType != EffectType.Nothing;
    public EffectType EffectType;
    public FloatReference FloatAmount = new FloatReference();
}
