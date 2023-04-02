using System.Collections.Generic;
using System.Linq;

public static class GearPackGeneration
{
    private const int _numAugmentsInPack = 5;
    private static readonly RarityFactors Factors = new DefaultRarityFactors();

    public static StaticEquipment[] GetRandomAugments(Hero currentHero, EquipmentPool gearPool, PartyAdventureState party)
    {
        var currentHeroGearIds = currentHero.Equipment.All.Select(e => e.Id).ToHashSet();
        
        var gearOptions = HeroPermanentAugmentOptions.GenerateHeroGearOptions(
                gearPool, party, currentHero.Character, 
                new HashSet<Rarity> { Rarity.Common, Rarity.Uncommon, Rarity.Rare, Rarity.Epic}, 10)
            .Where(g => !currentHeroGearIds.Contains(g.Id))
            .Take(_numAugmentsInPack);
        return gearOptions.ToArray();
    }

    public static StaticEquipment[] GetDualRarityAugmentPack(Hero currentHero, EquipmentPool gearPool, PartyAdventureState party)
    {
        var packRarityPicked = Enumerable.Range(0, 5)
            .Select(r => new Rarity[] { Rarity.Common, Rarity.Uncommon, Rarity.Rare, Rarity.Epic }.Random(Factors))
            .OrderByDescending(r => (int)r)
            .First();
        var packRarity = packRarityPicked == Rarity.Common ? Rarity.Uncommon : packRarityPicked;
        var selectedRarities = new HashSet<Rarity> { packRarity, (Rarity) ((int) packRarity - 1) };
        
        var currentHeroGearIds = currentHero.Equipment.All.Select(e => e.Id).ToHashSet();
        var gearOptions = HeroPermanentAugmentOptions.GenerateHeroGearOptions(
                gearPool, party, currentHero.Character, 
                selectedRarities, 10)
            .Where(g => !currentHeroGearIds.Contains(g.Id))
            .Take(_numAugmentsInPack);
        
        return gearOptions.ToArray();
    }
}