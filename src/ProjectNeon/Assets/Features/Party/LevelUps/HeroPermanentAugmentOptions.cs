using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUpsV4/HeroPermanentAugmentOptions")]
public class HeroPermanentAugmentOptions : LevelUpOptions
{
    [SerializeField] private EquipmentPool allEquipmentPool;
    [SerializeField] private Rarity rarity;
    [SerializeField] private EquipmentPresenter customPresenterPrototype;

    public override string ChoiceDescription => "Choose an Augment";

    public override LevelUpOption[] Generate(Hero h)
    {
        var archetypes = h.Archetypes;
        var possible = allEquipmentPool.Possible(EquipmentSlot.Augmentation, rarity, h.Archetypes).ToList();
        Log.Info($"{h.Name}: {possible.Count} Possible Level Up Augment Options - {string.Join(", ", possible.Select(e => e.Name))}");
        
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
            .Select(e => (LevelUpOption)new WithCustomPresenter(
                    new AugmentLevelUpOption(e), 
                        ctx => Instantiate(customPresenterPrototype, ctx.Parent)
                            .Initialized(e, () => Message.Publish(new LevelUpOptionSelected(ctx.Option, ctx.AllOptions)), true).gameObject))
            .ToArray()
            .Shuffled();
        
        return options;
    }
}
