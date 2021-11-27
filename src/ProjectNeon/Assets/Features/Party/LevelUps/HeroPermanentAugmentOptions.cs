using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUps/HeroPermanentAugmentOptions")]
public class HeroPermanentAugmentOptions : LevelUpOptions
{
    [SerializeField] private EquipmentPool allEquipmentPool;
    [SerializeField] private Rarity rarity;
    
    public override LevelUpOption[] Generate(Hero h)
    {
        var archetypes = h.Archetypes;
        var archMatchingAugment = allEquipmentPool.All
            .Where(e => e.Rarity == rarity 
                        && e.Slot == EquipmentSlot.Augmentation 
                        && e.Archetypes.Any(a => archetypes.Contains(a)))
            .TakeRandom(1);
        var additionalAugments = allEquipmentPool.Random(EquipmentSlot.Augmentation, rarity, h.Character.AsArray(), 3 - archMatchingAugment.Length);
        var options = archMatchingAugment
            .Concat(additionalAugments)
            .Select(a => (LevelUpOption)new AugmentLevelUpOption(a))
            .ToArray();
        return options.Shuffled();
    }
}
