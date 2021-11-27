using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUps/HeroPermanentAugmentOptions")]
public class HeroPermanentAugmentOptions : LevelUpOptions
{
    [SerializeField] private EquipmentPool allEquipmentPool;
    [SerializeField] private Rarity rarity;
    
    public override LevelUpOption[] Generate(Hero h)
    {
        var augments = allEquipmentPool.Random(EquipmentSlot.Augmentation, rarity, h.Character.AsArray(), 3);
        var options = augments.Select(a => (LevelUpOption)new AugmentLevelUpOption(a)).ToArray();
        return options;
    }
}