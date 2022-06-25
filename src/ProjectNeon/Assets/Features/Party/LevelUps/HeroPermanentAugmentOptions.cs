using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUpsV4/HeroPermanentAugmentOptions")]
public class HeroPermanentAugmentOptions : LevelUpOptions
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private EquipmentPool allEquipmentPool;
    [SerializeField] private Rarity rarity;
    [SerializeField] private EquipmentPresenter customPresenterPrototype;
    [SerializeField] private StaticEquipment[] choiceOverride;

    public override string ChoiceDescription => "Choose an Augment";

    public override LevelUpOption[] Generate(Hero h)
    {
        var finalSet = GenerateHeroGearOptions(allEquipmentPool, party, h.Character, new HashSet<Rarity> {rarity}, 3);
            if (choiceOverride.Length == 3)
            finalSet = choiceOverride.Cast<Equipment>().ToArray();
        
        // Create Options
        var options = finalSet
            .Select(e => (LevelUpOption)new WithCustomPresenter(
                    new AugmentLevelUpOption(party, e), 
                        ctx => Instantiate(customPresenterPrototype, ctx.Parent)
                            .Initialized(e, () => Message.Publish(new LevelUpOptionSelected(ctx.Option, ctx.AllOptions)), true).gameObject))
            .ToArray()
            .Shuffled();
        
        return options;
    }

    public static Equipment[] GenerateHeroGearOptions(EquipmentPool allEquipmentPool, PartyAdventureState party,
        HeroCharacter h, HashSet<Rarity> rarities, int count)
    {
        var archetypes = h.Archetypes;
        var possible = allEquipmentPool.Possible(EquipmentSlot.Augmentation, rarities, h.Archetypes, h.Stats.KeyStatTypes(), party.KeyStats).ToList();
        Log.Info($"{h.Name}: {possible.Count} Possible Level Up Augment Options - {string.Join(", ", possible.Select(e => e.Name))}");
        
        // Pick one augment matching the character's Archetypes
        var archMatchingAugment = allEquipmentPool.All
            .Where(e => rarities.Contains(e.Rarity) 
                        && e.Slot == EquipmentSlot.Augmentation 
                        && e.Archetypes.Any(a => archetypes.Contains(a)))
            .TakeRandom(1);
        
        // Randoms might include the Archetype Matching Augment, so we need one extra
        var additionalAugments = allEquipmentPool.Random(EquipmentSlot.Augmentation, rarities, h.AsArray(), count);

        // Only Take Requested Count
        var finalSet = archMatchingAugment
            .Concat(additionalAugments)
            .DistinctBy(a => a.Description)
            .Take(count);

        return finalSet.ToArray();
    }
}
