using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Draft/WildLevelUpOptions")]
public class DraftWildLevelUpOptions : LevelUpOptions
{    
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private EquipmentPresenter equipmentPresenterPrototype;
    [SerializeField] private EquipmentPool allEquipmentPool;
    [SerializeField] private StaticHeroLevelUpOptions permanentEquipmentPool;
    [SerializeField] private int augmentWeight = 2;
    [SerializeField] private int permanentEquipmentWeight = 4;
    [SerializeField] private int statGainWeight = 8;

    public override string ChoiceDescription => "Select an Option!";
    
    public override LevelUpOption[] Generate(Hero h)
    {
        var numOptions = 3;
        var pickTypes = Num(DraftWildType.Augment, augmentWeight)
            .Concat(Num(DraftWildType.Equipment, permanentEquipmentWeight))
            .Concat(Num(DraftWildType.StatGain, statGainWeight))
            .ToArray()
            .Shuffled()
            .Take(numOptions);

        var statGains = DraftModeLevelUpRewardGenerator.GenerateOptions(h).ToQueue();
        var augmentsGear = HeroPermanentAugmentOptions.GenerateHeroGearOptions(allEquipmentPool, party, h.Character, new HashSet<Rarity>
            { Rarity.Common, Rarity.Uncommon, Rarity.Rare, Rarity.Epic}, 3);
        var augments = HeroPermanentAugmentOptions.ToLevelUpOptions(h, augmentsGear, party, equipmentPresenterPrototype).ToQueue();
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
}
