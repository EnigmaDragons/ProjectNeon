using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUpsV4/HeroPermanentAugmentOptions")]
public class HeroPermanentAugmentOptions : LevelUpOptions
{
    [SerializeField] private EquipmentPool allEquipmentPool;
    [SerializeField] private Rarity rarity;
    
    public override LevelUpOption[] Generate(Hero h)
    {
        var archetypes = h.Archetypes;
        Log.Info($"Possible Level Up Augment Options - {allEquipmentPool.PossibleCount(EquipmentSlot.Augmentation, rarity, h.Archetypes)}");
        
        // Pick one augment matching the character's Archetypes
        var archMatchingAugment = allEquipmentPool.All
            .Where(e => e.Rarity == rarity 
                        && e.Slot == EquipmentSlot.Augmentation 
                        && e.Archetypes.Any(a => archetypes.Contains(a)))
            .TakeRandom(1);
        
        // Randoms might include the Archetype Matching Augment, so we need one extra
        var additionalAugments = allEquipmentPool.Random(EquipmentSlot.Augmentation, rarity, h.Character.AsArray(), 3);

        // Only Take 3
        var finalSet = archMatchingAugment
            .Concat(additionalAugments)
            .DistinctBy(a => a.Description)
            .Take(3);
        
        // Create Options
        var options = finalSet
            .Select(a => (LevelUpOption)new AugmentLevelUpOption(a))
            .ToArray()
            .Shuffled();
        
        return options;
    }
}
