using System;

[Serializable]
public sealed class EffectData
{
    public bool ShouldApply => EffectType != EffectType.Nothing;
    public EffectType EffectType;
    public FloatReference FloatAmount = new FloatReference();
    public IntReference NumberOfTurns = new IntReference();
    public StringReference EffectScope = new StringReference();
    public int IntAmount => Convert.ToInt32(Math.Ceiling(FloatAmount.Value));
}
