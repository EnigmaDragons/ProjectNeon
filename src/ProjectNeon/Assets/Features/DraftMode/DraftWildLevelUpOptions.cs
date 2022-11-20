using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Draft/WildLevelUpOptions")]
public class DraftWildLevelUpOptions : LevelUpOptions, ILocalizeTerms
{    
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private EquipmentPresenter equipmentPresenterPrototype;
    [SerializeField] private EquipmentPool allEquipmentPool;
    [SerializeField] private StaticHeroLevelUpOptions permanentEquipmentPool;
    [SerializeField] private StageRarityFactors augmentRarityFactors;
    [SerializeField] private int augmentWeight = 2;
    [SerializeField] private int permanentEquipmentWeight = 4;
    [SerializeField] private int statGainWeight = 8;

    public override string ChoiceDescriptionTerm => "LevelUps/SelectOption";
    
    public override LevelUpOption[] Generate(Hero h)
    {
        var numOptions = 3;
        var pickTypes = Num(DraftWildType.Equipment, permanentEquipmentWeight)
            .Concat(Num(DraftWildType.StatGain, statGainWeight))
            .Concat(h.Equipment.Augments.Length == 6 ? new DraftWildType[0] : Num(DraftWildType.Augment, augmentWeight))
            .ToArray()
            .Shuffled()
            .Take(numOptions);

        var statGains = DraftModeLevelUpRewardGenerator.GenerateOptions(h).ToQueue();
        var rarities = 
            Enumerable.Range(0, augmentRarityFactors[Rarity.Common]).Select(_ => Rarity.Common).Concat(
            Enumerable.Range(0, augmentRarityFactors[Rarity.Uncommon]).Select(_ => Rarity.Uncommon)).Concat(
            Enumerable.Range(0, augmentRarityFactors[Rarity.Rare]).Select(_ => Rarity.Rare)).Concat(
            Enumerable.Range(0, augmentRarityFactors[Rarity.Epic]).Select(_ => Rarity.Epic));
        var augmentGear = allEquipmentPool.Random(EquipmentSlot.Augmentation, rarities.Random(), new[] {h.Character}, 1);
        var augments = HeroPermanentAugmentOptions.ToLevelUpOptions(h, augmentGear.ToArray(), party, equipmentPresenterPrototype).ToQueue();
        var equipments = permanentEquipmentPool.options.Take(3).ToQueue();

        var options = pickTypes.Select(t =>
        {
            if (t == DraftWildType.Augment)
                return augments.Dequeue();
            if (t == DraftWildType.Equipment)
                return equipments.Dequeue();
            return statGains.Dequeue();
        }).ToArray();
        
        return options;
    }

    private DraftWildType[] Num(DraftWildType type, int count) 
        => Enumerable.Range(0, count).Select(_ => type).ToArray();

    private enum DraftWildType
    {
        Augment,
        Equipment,
        StatGain
    }

    public string[] GetLocalizeTerms()
        => new[] {ChoiceDescriptionTerm};
}
