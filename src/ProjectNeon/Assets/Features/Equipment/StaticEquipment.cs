using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Equipment", order = -80)]
public class StaticEquipment : ScriptableObject, Equipment
{
    [SerializeField, UnityEngine.UI.Extensions.ReadOnly] public int id;
    [SerializeField] private string displayName;
    [SerializeField] private string description;
    [SerializeField] private StringVariable corp;
    [SerializeField] private StringVariable[] archetypes = new StringVariable[0];
    [SerializeField] private Rarity rarity;
    [SerializeField] private EquipmentSlot slot;
    [SerializeField] private float priceFactor = 1f;
    [SerializeField] private EquipmentStatModifier[] modifiers = new EquipmentStatModifier[0];
    [SerializeField] private ResourceTypeModifications[] resourceModifiers = new ResourceTypeModifications[0];
    [SerializeField] private EffectData[] turnStartEffects = new EffectData[0];
    [SerializeField] private EffectData[] turnEndEffects = new EffectData[0];
    [SerializeField] private EffectData[] battleStartEffects = new EffectData[0];
    [SerializeField] private bool isWIP;
    
    public string Name => !string.IsNullOrWhiteSpace(displayName) 
        ? displayName 
        : name.SkipThroughFirstDash().SkipThroughFirstUnderscore().WithSpaceBetweenWords();

    public int Id => id;
    public string Description => description;
    public bool IsWip => isWIP;
    public int Price => CardShopPricing.EquipmentShopPrice(rarity, priceFactor);
    public Rarity Rarity => rarity;
    public string[] Archetypes => archetypes.Select(a => a.Value).ToArray();
    public string ArchetypeKey => string.Join(" + ", Archetypes.OrderBy(a => a));
    public EquipmentSlot Slot => slot;
    public EquipmentStatModifier[] Modifiers => modifiers.ToArray();
    public IResourceType[] ResourceModifiers => resourceModifiers;
    public EffectData[] TurnStartEffects => turnStartEffects;
    public EffectData[] TurnEndEffects => turnEndEffects;
    public EffectData[] BattleStartEffects => battleStartEffects;
    public string Corp => corp == null ? "No Corp" : corp.Value;
    
    public GameEquipmentData GetData() 
        => new GameEquipmentData { Type = GameEquipmentDataType.StaticEquipmentId, StaticEquipmentId = id };

    public IEnumerable<EffectData> AllEffects => TurnStartEffects.Concat(TurnEndEffects).Concat(BattleStartEffects);

    public IStats AdditiveStats()
    {
        var stats = new StatAddends();
        modifiers.Where(x => x.ModifierType == StatMathOperator.Additive)
            .ForEach(m => stats.WithRaw(m.StatType, m.Amount));
        return stats;
    }
}
