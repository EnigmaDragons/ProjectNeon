using System;

[Serializable]
public class GlobalEffectData
{
    public string ShortDescription;
    public string FullDescription;
    public GlobalEffectType EffectType = GlobalEffectType.None;

    public FloatReference FloatAmount = new FloatReference(0);
    public int IntAmount => FloatAmount.Value.CeilingInt();

    public TargetedEffectData BattleEffect;

    public StringReference CorpName;
    public StringReference EffectScope;
}
