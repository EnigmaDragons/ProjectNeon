using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/Equipment")]
public class StaticEquipment : ScriptableObject, Equipment
{
    [SerializeField] private string displayName;
    [SerializeField] private string description;
    [SerializeField] private Rarity rarity;
    [SerializeField] private EquipmentSlot slot;
    [SerializeField] private float priceFactor = 1f;
    [SerializeField] private StringVariable[] archetypes = new StringVariable[0];
    [SerializeField] private EquipmentStatModifier[] modifiers = new EquipmentStatModifier[1];
    [SerializeField] private ResourceTypeModifications[] resourceModifiers = new ResourceTypeModifications[0];
    [SerializeField] private EffectData[] turnStartEffects = new EffectData[0];
    [SerializeField] private EffectData[] turnEndEffects = new EffectData[0];
    [SerializeField] private EffectData[] battleStartEffects = new EffectData[0];
    
    public string Name => !string.IsNullOrWhiteSpace(displayName) 
        ? displayName 
        : name.SkipThroughFirstDash().SkipThroughFirstUnderscore().WithSpaceBetweenWords();
    
    public string Description => description;
    public int Price => CardShopPricing.EquipmentShopPrice(rarity, priceFactor);
    public Rarity Rarity => rarity;
    public string[] Archetypes => archetypes.Select(a => a.Value).ToArray();
    public EquipmentSlot Slot => slot;
    public EquipmentStatModifier[] Modifiers => modifiers.ToArray();
    public IResourceType[] ResourceModifiers => resourceModifiers;
    public EffectData[] TurnStartEffects => turnStartEffects;
    public EffectData[] TurnEndEffects => turnEndEffects;
    public EffectData[] BattleStartEffects => battleStartEffects;
    public IEnumerable<EffectData> AllEffects => TurnStartEffects.Concat(TurnEndEffects).Concat(BattleStartEffects);

    public IStats AdditiveStats()
    {
        var stats = new StatAddends();
        modifiers.Where(x => x.ModifierType == StatMathOperator.Additive)
            .ForEach(m => stats.WithRaw(m.StatType, m.Amount));
        return stats;
    }
}
