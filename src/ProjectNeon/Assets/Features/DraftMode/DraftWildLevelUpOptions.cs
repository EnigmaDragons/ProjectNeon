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
    [SerializeField] private DeterminedNodeInfo nodeInfo;
    [SerializeField] private Library library;
    [SerializeField] private int augmentWeight = 2;
    [SerializeField] private int permanentEquipmentWeight = 4;
    [SerializeField] private int statGainWeight = 8;

    public override string ChoiceDescriptionTerm => "LevelUps/SelectOption";
    
    public override LevelUpOption[] Generate(Hero h)
    {
        if (nodeInfo.DraftLevelUpOptions.IsMissing)
        {
            var numOptions = 3;
            var pickTypes = Num(DraftWildType.Equipment, permanentEquipmentWeight)
                .Concat(Num(DraftWildType.StatGain, statGainWeight))
                .Concat(h.Equipment.Augments.Length == 6 ? new DraftWildType[0] : Num(DraftWildType.Augment, augmentWeight))
                .ToArray()
                .Shuffled()
                .Take(numOptions);

            var statGains = DraftModeLevelUpRewardGenerator.GenerateOptions().ToQueue();
            var rarities = 
                Enumerable.Range(0, augmentRarityFactors[Rarity.Common]).Select(_ => Rarity.Common).Concat(
                    Enumerable.Range(0, augmentRarityFactors[Rarity.Uncommon]).Select(_ => Rarity.Uncommon)).Concat(
                    Enumerable.Range(0, augmentRarityFactors[Rarity.Rare]).Select(_ => Rarity.Rare)).Concat(
                    Enumerable.Range(0, augmentRarityFactors[Rarity.Epic]).Select(_ => Rarity.Epic));
            var augmentGear = allEquipmentPool.Random(EquipmentSlot.Augmentation, rarities.Random(), new[] {h.Character}, 1).ToQueue();
            var equipments = permanentEquipmentPool.options.Take(3).ToQueue();

            nodeInfo.DraftLevelUpOptions = pickTypes.Select(t =>
            {
                if (t == DraftWildType.Augment)
                    return new DraftWildLevelUpData {Type = DraftWildType.Augment, GearId = augmentGear.Dequeue().Id};
                if (t == DraftWildType.Equipment)
                    return new DraftWildLevelUpData {Type = DraftWildType.Equipment, StaticOptionId = equipments.Dequeue().Id};
                return new DraftWildLevelUpData {Type = DraftWildType.StatGain, Stat = statGains.Dequeue()};
            }).ToArray();
            Message.Publish(new SaveDeterminationsRequested());
        }
        
        var options = nodeInfo.DraftLevelUpOptions.Value.Select(x =>
        {
            if (x.Type == DraftWildType.Augment)
                return HeroPermanentAugmentOptions.ToLevelUpOption(h, library.GetEquipment(new GameEquipmentData { StaticEquipmentId = x.GearId.Value }).Value, party, equipmentPresenterPrototype);
            if (x.Type == DraftWildType.Equipment)
                return permanentEquipmentPool.options.First(option => option.Id == x.StaticOptionId.Value);
            if (x.Type == DraftWildType.StatGain)
                return DraftModeLevelUpRewardGenerator.CreateLevelUpOption(h, x.Stat.Value);
            Log.Error("Draft Wild Level Up Options had an impossible error");
            return null;
        }).ToArray();
        
        return options;
    }

    private DraftWildType[] Num(DraftWildType type, int count) 
        => Enumerable.Range(0, count).Select(_ => type).ToArray();

    public string[] GetLocalizeTerms()
        => new[] {ChoiceDescriptionTerm};
}
