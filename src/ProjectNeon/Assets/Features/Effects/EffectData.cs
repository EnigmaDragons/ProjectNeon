using System;
using UnityEngine;

[Serializable]
public sealed class EffectData
{
    public static readonly EffectData Nothing = new EffectData(); 
    
    public bool ShouldApply => EffectType != EffectType.Nothing;
    public EffectType EffectType;
    public FloatReference FloatAmount = new FloatReference();
    public IntReference NumberOfTurns = new IntReference();
    public StringReference EffectScope = new StringReference();
    public int IntAmount => Mathf.CeilToInt(FloatAmount.Value);
    public EffectData origin = Nothing;
    public int TargetIndex = 0;
}
