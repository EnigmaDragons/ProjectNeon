using System;
using System.Collections.Generic;
using System.Linq;

public class ShopSelectionPicker
{
    private readonly RarityFactors factors;
    private readonly PartyAdventureState party;

    public ShopSelectionPicker(RarityFactors factors, PartyAdventureState party)
    {
        this.factors = factors;
        this.party = party;
    }
    
    // TODO Move Card and Equipment Pools to Ctor
    public CardType[] PickCards(ShopCardPool cards, int numCards, params Rarity[] rarities)
    {
        var partyClasses = new HashSet<string>(party.BaseHeroes.Select(h => h.Class.Name).Concat(CharacterClass.All));

        var weightedCards = cards
            .AllExceptStarters
            .Where(x => x.LimitedToClass.IsMissingOr(c => partyClasses.Contains(c.Name)))
            .Where(x => rarities.None() || rarities.Contains(x.Rarity))
            .FactoredByRarity(factors, x => x.Rarity)
            .ToArray()
            .Shuffled();

        var selectedCards = new HashSet<CardType>();
        for (var i = 0; i < weightedCards.Length && selectedCards.Count < numCards; i++)
            selectedCards.Add(weightedCards[i]);
        
        return selectedCards.ToArray();
    }

    public Equipment[] PickEquipments(EquipmentPool equipmentPool, int numEquipment,
        params Rarity[] rarities)
    {
        rarities = rarities.None() ? new[] {Rarity.Common, Rarity.Uncommon, Rarity.Rare, Rarity.Epic} : rarities;
        var partyClasses = new HashSet<string>(party.BaseHeroes.Select(h => h.Class.Name).Concat(CharacterClass.All));
        var randomRarities = rarities.Random(factors, numEquipment).ToArray();
        var randomSlots = equipmentPool.Random(numEquipment).ToArray();
        var groups = new Dictionary<Rarity, Dictionary<EquipmentSlot, int>>();
        Enumerable.Range(0, numEquipment).ForEach(i =>
        {
            var rarity = randomRarities[i];
            var slot = randomSlots[i];
            if (!groups.ContainsKey(rarity))
                groups[rarity] = new Dictionary<EquipmentSlot, int>();
            if (!groups[rarity].ContainsKey(slot))
                groups[rarity][slot] = 0;
            groups[rarity][slot]++;
        });
        return groups.SelectMany(r => r.Value.SelectMany(s => equipmentPool.Random(s.Key, r.Key, partyClasses, s.Value))).ToArray();
    }

    public ShopSelection GenerateCardSelection(ShopCardPool cards, int numCards)
    {
        var selectedCards = PickCards(cards, 1, Rarity.Rare, Rarity.Epic)
            .Concat(PickCards(cards, 2, Rarity.Uncommon))
            .Concat(PickCards(cards, numCards - 3, Rarity.Common))
            .ToArray()
            .Shuffled();
        return new ShopSelection(new List<Equipment>(), selectedCards.ToList());
    }
    
    public ShopSelection GenerateEquipmentSelection(EquipmentPool equipment, int numEquips)
    {
        var selectedEquipment = PickEquipments(equipment, 1, Rarity.Rare, Rarity.Epic)
            .Concat(PickEquipments(equipment, 2, Rarity.Uncommon))
            .Concat(PickEquipments(equipment, numEquips - 3, Rarity.Common))
            .ToArray()
            .Shuffled();
        return new ShopSelection(selectedEquipment.ToList(), new List<CardType>());
    }

    [Obsolete("V1 Shops")]
    public ShopSelection GenerateV1MixedShopSelection(ShopCardPool cards, EquipmentPool equipment)
    {
        var selectedCards = PickCards(cards, 4);
        var selectedEquipment = PickEquipments(equipment, 4);
        
        return new ShopSelection(selectedEquipment.ToList(), selectedCards.ToList());
    }
}
