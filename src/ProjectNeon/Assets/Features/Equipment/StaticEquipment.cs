using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Equipment", order = -80)]
public class StaticEquipment : ScriptableObject, Equipment, ILocalizeTerms
{
    [SerializeField, UnityEngine.UI.Extensions.ReadOnly] public int id;
    [SerializeField] private string displayName;
    [SerializeField] public string description;
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
    [SerializeField] private EffectData[] battleEndEffects = new EffectData[0];
    [SerializeField] private StatType[] requiresStatType = new StatType[0];
    [SerializeField] private StatType[] excludeIfPartyHasStatType = new StatType[0];
    [SerializeField] private bool isWIP = true;
    [SerializeField] private bool notAvailableForGeneralDistribution = false;
    [SerializeField] private string designComment;
    
    public string Name => !string.IsNullOrWhiteSpace(displayName) 
        ? displayName 
        : name.SkipThroughFirstDash().SkipThroughFirstUnderscore().WithSpaceBetweenWords();

    public int Id => id;
    public string Description => description;
    public bool IsWip => isWIP;
    public bool IncludeInPools => !isWIP && !notAvailableForGeneralDistribution;
    public int Price => CardShopPricing.EquipmentShopPrice(rarity, priceFactor);
    public Rarity Rarity => rarity;
    public string[] Archetypes => archetypes == null ? Array.Empty<string>() : archetypes.Where(a => a != null).Select(a => a.Value).ToArray();
    public string ArchetypeKey => string.Join(" + ", Archetypes.OrderBy(a => a));
    public EquipmentSlot Slot => slot;
    public EquipmentStatModifier[] Modifiers => modifiers.ToArray();
    public IResourceType[] ResourceModifiers => resourceModifiers;
    public EffectData[] TurnStartEffects => turnStartEffects;
    public EffectData[] TurnEndEffects => turnEndEffects;
    public EffectData[] BattleStartEffects => battleStartEffects;
    public EffectData[] BattleEndEffects => battleEndEffects;
    public string Corp => corp == null ? "No Corp" : corp.Value;
    public EquipmentDistributionRules DistributionRules => 
        new EquipmentDistributionRules
        {
            RequiresStatType = requiresStatType, 
            ExcludeIfPartyHasStatType = excludeIfPartyHasStatType
        };

    public Maybe<CardTypeData> ReferencedCard => TurnStartEffects.Concat(TurnEndEffects).Concat(BattleStartEffects)
        .Select(e => e.BonusCardType)
        .Cast<CardTypeData>()
        .Where(e => e != null)
        .FirstAsMaybe();

    private static string WipWord(bool isWip) => isWip ? "WIP - " : "";
    public string EditorName => $"{WipWord(IsWip)}{Rarity} - {Name}";

    public GameEquipmentData GetData() 
        => new GameEquipmentData { Type = GameEquipmentDataType.StaticEquipmentId, StaticEquipmentId = id };

    public IEnumerable<EffectData> AllEffects => TurnStartEffects.Concat(TurnEndEffects).Concat(BattleStartEffects).Concat(BattleEndEffects);

    public IStats AdditiveStats()
    {
        var stats = new StatAddends();
        modifiers.Where(x => x.ModifierType == StatMathOperator.Additive)
            .Where(x => !string.IsNullOrEmpty(x.StatType))
            .ForEach(m => stats.WithRaw(m.StatType, m.Amount));
        return stats;
    }

    public string[] GetLocalizeTerms()
        => !IsWip && (Slot == EquipmentSlot.Augmentation || Slot == EquipmentSlot.Permanent)
            ? new[] {this.LocalizationNameTerm(), this.LocalizationDescriptionTerm()}
                .Concat(AllEffects.Where(x => x.StatusTag != StatusTag.None || x.EffectType == EffectType.OnDeath).Select(x => x.StatusDetailTerm))
                .ToArray()
            : Array.Empty<string>();
}
