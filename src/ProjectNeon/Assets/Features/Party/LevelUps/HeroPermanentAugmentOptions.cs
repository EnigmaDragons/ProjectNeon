using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUpsV4/HeroPermanentAugmentOptions")]
public class HeroPermanentAugmentOptions : LevelUpOptions, ILocalizeTerms
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private EquipmentPool allEquipmentPool;
    [SerializeField] private Rarity rarity;
    [SerializeField] private EquipmentPresenter customPresenterPrototype;
    [SerializeField] private StaticEquipment[] choiceOverride;
    [SerializeField] private DeterminedNodeInfo nodeInfo;

    public override string ChoiceDescriptionTerm => "LevelUps/ChooseAugment";

    public override LevelUpOption[] Generate(Hero h)
    {
        if (choiceOverride.Length == 3)
            return ToLevelUpOptions(h, choiceOverride.ToArray(), party, customPresenterPrototype);
        if (nodeInfo.HeroLevelUpAugments.IsMissing)
        {
            nodeInfo.HeroLevelUpAugments = GenerateHeroGearOptions(allEquipmentPool, party, h.Character, new HashSet<Rarity> {rarity}, 3);
            Message.Publish(new SaveDeterminationsRequested());
        }
        return ToLevelUpOptions(h, nodeInfo.HeroLevelUpAugments.Value, party, customPresenterPrototype);
    }

    private LevelUpOption[] ToLevelUpOptions(Hero h, StaticEquipment[] equipments, PartyAdventureState party, EquipmentPresenter customPresenterPrototype)
    {
        return equipments
            .Select(e => ToLevelUpOption(h, e, party, customPresenterPrototype))
            .ToArray()
            .Shuffled();
    }
    
    public static LevelUpOption ToLevelUpOption(Hero h, StaticEquipment equipment, PartyAdventureState party, EquipmentPresenter customPresenterPrototype)
    {
        return new WithCustomPresenter(
            new AugmentLevelUpOption(party, equipment), 
            ctx => Instantiate(customPresenterPrototype, ctx.Parent)
                .Initialized(equipment, () => Message.Publish(new LevelUpOptionSelected(ctx.Option, ctx.AllOptions)), true).gameObject);
    }
    
    public static StaticEquipment[] GenerateHeroGearOptions(EquipmentPool allEquipmentPool, PartyAdventureState party,
        BaseHero h, HashSet<Rarity> rarities, int count)
    {
        var archetypes = h.Archetypes;
        var possible = allEquipmentPool.Possible(EquipmentSlot.Augmentation, rarities, h.Archetypes, h.Stats.KeyStatTypes(), party.KeyStats).ToList();
        Log.Info($"{h.NameTerm().ToEnglish()}: {possible.Count} Possible Level Up Augment Options - {string.Join(", ", possible.Select(e => e.Name))}");
        
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

    public string[] GetLocalizeTerms()
        => new[] {ChoiceDescriptionTerm};
}
