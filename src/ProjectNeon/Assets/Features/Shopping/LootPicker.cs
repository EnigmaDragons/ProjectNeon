using System.Collections.Generic;
using System.Linq;

public class LootPicker
{
    private readonly int stage;
    private readonly RarityFactors factors;
    private readonly PartyAdventureState party;
    private readonly DeterministicRng rng;

    public LootPicker(int stage, RarityFactors factors, PartyAdventureState party)
        : this(stage, factors, party, new DeterministicRng(Rng.NewSeed())) {}
    
    public LootPicker(int stage, RarityFactors factors, PartyAdventureState party, DeterministicRng rng)
    {
        this.stage = stage;
        this.factors = factors;
        this.party = party;
        this.rng = rng;
    }

    public Rarity RandomRarity() => factors.Random();
    
    // Factor out duplication after more thinking. Be careful not to combine all archetypes across heroes.
    public CardTypeData[] PickCardsForSingleHero(ShopCardPool cards, HashSet<string> archetypes, int numCards, params Rarity[] rarities)
    {
        var weightedCards = cards.Get(archetypes, party.CardsYouCantHaveMoreOf(), rarities)
            .Distinct()
            .FactoredByRarity(factors, x => x.Rarity)
            .ToArray()
            .Shuffled();
    
        var selectedCards = new HashSet<CardTypeData>();
        for (var i = 0; i < weightedCards.Length && selectedCards.Count < numCards; i++)
            selectedCards.Add(weightedCards[i]);
        
        return selectedCards.ToArray();
    }

    public CardTypeData[] PickCards(ShopCardPool cards, int numCards, params Rarity[] rarities)
    {
        var weightedCards = party.BaseHeroes.SelectMany(h => cards.Get(h.Archetypes, party.CardsYouCantHaveMoreOf(), rarities))
            .Distinct()
            .FactoredByRarity(factors, x => x.Rarity)
            .ToArray()
            .Shuffled();

        var selectedCards = new HashSet<CardTypeData>();
        for (var i = 0; i < weightedCards.Length && selectedCards.Count < numCards; i++)
            selectedCards.Add(weightedCards[i]);
        
        return selectedCards.ToArray();
    }

    public Equipment[] PickEquipments(EquipmentPool equipmentPool, int numEquipment, params Rarity[] rarities)
        => PickEquipments(equipmentPool, numEquipment, "", rarities);
    
    public Equipment[] PickEquipments(EquipmentPool equipmentPool, int numEquipment, string corpName,
        params Rarity[] rarities)
    {
        rarities = rarities.None() ? new[] { Rarity.Common, Rarity.Uncommon, Rarity.Rare, Rarity.Epic } : rarities;
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
        return groups.SelectMany(r => r.Value.SelectMany(s => equipmentPool.Random(s.Key, r.Key, party.BaseHeroes, s.Value, corpName))).ToArray();
    }

    public ShopSelection GenerateCardSelection(ShopCardPool cards, int numCards)
    {
        CardTypeData[] selectedCards;
        if (stage > 1)
        {
            selectedCards = PickCards(cards, 2, Rarity.Rare, Rarity.Epic)
                .Concat(PickCards(cards, 3, Rarity.Uncommon))
                .Concat(PickCards(cards, numCards - 5, Rarity.Common))
                .ToArray()
                .Shuffled(rng);
        }
        else
        {            
            selectedCards = PickCards(cards, 1, Rarity.Rare, Rarity.Epic)
                .Concat(PickCards(cards, 2, Rarity.Uncommon))
                .Concat(PickCards(cards, numCards - 3, Rarity.Common))
                .ToArray()
                .Shuffled(rng);
        }

        return new ShopSelection(new List<Equipment>(), selectedCards.ToList());
    }
    
    public ShopSelection GenerateEquipmentSelection(EquipmentPool equipment, int numEquips, string corpNameFilter)
    {
        Equipment[] selectedEquipment;
        if (stage > 1)
        {
            selectedEquipment = PickEquipments(equipment, 2, corpNameFilter, Rarity.Rare, Rarity.Epic)
                .Concat(PickEquipments(equipment, 3, corpNameFilter, Rarity.Uncommon))
                .Concat(PickEquipments(equipment, numEquips - 5, corpNameFilter, Rarity.Common))
                .ToArray()
                .Shuffled();
        }
        else
        {
            selectedEquipment = PickEquipments(equipment, 1, corpNameFilter, Rarity.Rare, Rarity.Epic)
                .Concat(PickEquipments(equipment, 2, corpNameFilter, Rarity.Uncommon))
                .Concat(PickEquipments(equipment, numEquips - 3, corpNameFilter, Rarity.Common))
                .ToArray()
                .Shuffled();
        }
        if (selectedEquipment.Length < numEquips)
            Log.Warn($"{corpNameFilter} doesn't have enough augments");
        while (selectedEquipment.Length < numEquips)
        {
            selectedEquipment = selectedEquipment.Concat(PickEquipments(equipment, 1, corpNameFilter, Rarity.Common)).ToArray();
        }
        return new ShopSelection(selectedEquipment.ToList(), new List<CardTypeData>());
    }
}
