using System.Collections.Generic;
using System.Linq;

public class ShopSelectionPicker
{
    private static readonly int NumCards = 4;
    private static readonly int NumEquipment = 4;
    
    public CardType[] PickCards(PartyAdventureState party, ShopCardPool cards, int numCards, params Rarity[] rarities)
    {
        var partyClasses = new HashSet<string>(party.BaseHeroes.Select(h => h.Class.Name).Concat(CharacterClass.All));

        var weightedCards = cards
            .AllExceptStarters
            .Where(x => x.LimitedToClass.IsMissingOr(c => partyClasses.Contains(c.Name)))
            .Where(x => rarities.None() || rarities.Contains(x.Rarity))
            .FactoredByRarity(x => x.Rarity)
            .ToArray()
            .Shuffled();

        var selectedCards = new HashSet<CardType>();
        for (var i = 0; i < weightedCards.Length && selectedCards.Count < numCards; i++)
            selectedCards.Add(weightedCards[i]);
        
        return selectedCards.ToArray();
    }

    public ShopSelection GenerateCardSelection(PartyAdventureState party, ShopCardPool cards, int numCards)
    {
        var selectedCards = PickCards(party, cards, 1, Rarity.Rare, Rarity.Epic)
            .Concat(PickCards(party, cards, 2, Rarity.Uncommon))
            .Concat(PickCards(party, cards, numCards - 3, Rarity.Common))
            .ToArray()
            .Shuffled();
        return new ShopSelection(new List<Equipment>(), selectedCards.ToList());
    }
    
    public ShopSelection GenerateEquipmentSelection(EquipmentPool equipment, PartyAdventureState party, int numEquips)
    {
        var selectedEquipment = PickEquipment(party, equipment, 1, Rarity.Rare, Rarity.Epic)
            .Concat(PickEquipment(party, equipment, 2, Rarity.Uncommon))
            .Concat(PickEquipment(party, equipment, numEquips - 3, Rarity.Common))
            .ToArray()
            .Shuffled();
        return new ShopSelection(selectedEquipment.ToList(), new List<CardType>());
    }

    private static HashSet<Equipment> PickEquipment(PartyAdventureState party, EquipmentPool equipment, int numEquips, params Rarity[] rarities)
    {
        var partyClasses = new HashSet<string>(party.BaseHeroes.Select(h => h.Class.Name).Concat(CharacterClass.All));
        
        var weightedEquipment = equipment
            .All
            .Where(x => x.Classes.Any(c => partyClasses.Contains(c)))
            .Where(x => rarities.None() || rarities.Contains(x.Rarity))
            .FactoredByRarity(x => x.Rarity)
            .ToArray()
            .Shuffled();

        var selectedEquipment = new HashSet<Equipment>();
        for (var i = 0; i < weightedEquipment.Length && selectedEquipment.Count < numEquips; i++)
            selectedEquipment.Add(weightedEquipment[i]);
        return selectedEquipment;
    }

    public ShopSelection GenerateSelection(ShopCardPool cards, EquipmentPool equipment, PartyAdventureState party)
    {
        var selectedCards = PickCards(party, cards, NumCards);
        var selectedEquipment = PickEquipment(party, equipment, NumEquipment);
        
        return new ShopSelection(selectedEquipment.ToList(), selectedCards.ToList());
    }
}
