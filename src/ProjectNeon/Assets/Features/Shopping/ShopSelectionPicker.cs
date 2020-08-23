using System.Collections.Generic;
using System.Linq;

public class ShopSelectionPicker
{
    private static readonly int NumCards = 4;
    private static readonly int NumEquipment = 2;

    public CardType[] PickCards(PartyAdventureState party, ShopCardPool cards, int numCards)
    {
        var partyClasses = new HashSet<string>(party.BaseHeroes.Select(h => h.Class.Name).Concat(CharacterClass.All));

        var weightedCards = cards
            .All
            .Where(x => x.LimitedToClass.IsMissingOr(c => partyClasses.Contains(c.Name)))
            .FactoredByRarity(x => x.Rarity)
            .ToArray()
            .Shuffled();

        var selectedCards = new HashSet<CardType>();
        for (var i = 0; i < weightedCards.Length && selectedCards.Count < numCards; i++)
            selectedCards.Add(weightedCards[i]);
        
        return selectedCards.ToArray();
    }
    
    public ShopSelection GenerateSelection(ShopCardPool cards, EquipmentPool equipment, PartyAdventureState party)
    {
        var partyClasses = new HashSet<string>(party.BaseHeroes.Select(h => h.Class.Name).Concat(CharacterClass.All));

        var selectedCards = PickCards(party, cards, NumCards);

        var weightedEquipment = equipment
            .All
            .Where(x => x.Classes.Any(c => partyClasses.Contains(c)))
            .FactoredByRarity(x => x.Rarity)
            .ToArray()
            .Shuffled();
        
        var selectedEquipment = new HashSet<Equipment>();
        for (var i = 0; i < weightedEquipment.Length && selectedEquipment.Count < NumEquipment; i++)
            selectedEquipment.Add(weightedEquipment[i]);
        
        return new ShopSelection(selectedEquipment.ToList(), selectedCards.ToList());
    }
}
