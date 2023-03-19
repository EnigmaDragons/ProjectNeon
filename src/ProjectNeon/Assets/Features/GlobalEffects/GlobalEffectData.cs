using System;

[Serializable]
public class GlobalEffectData
{
    [UnityEngine.UI.Extensions.ReadOnly] public int OriginatingId = -1;
    
    public string ShortDescriptionTerm => $"GlobalEffects/Effect{OriginatingId}ShortDescription";
    public string FullDescriptionTerm => $"GlobalEffects/Effect{OriginatingId}FullDescription";
    public string ShortDescription;
    public string FullDescription;
    public GlobalEffectType EffectType = GlobalEffectType.None;

    public FloatReference FloatAmount = new FloatReference(0);
    public int IntAmount => FloatAmount.Value.CeilingInt();

    public TargetedEffectData BattleEffect;

    public StringReference CorpName;
    public StringReference EffectScope;

    public StaticEquipment Equipment;

    public CardType[] Cards;

    public GlobalEffectData WithOriginatingId(int id)
    {
        OriginatingId = id;
        return this;
    }
}
