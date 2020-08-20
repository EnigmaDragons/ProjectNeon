using System.Collections.Generic;
using System.Linq;

public class ShopSelectionPicker
{
    private static readonly int NumCards = 4;
    private static readonly int NumEquipment = 2;
    
    public ShopSelection GenerateSelection(ShopCardPool cards, EquipmentPool equipment, PartyAdventureState party)
    {
        var partyClasses = new HashSet<string>(party.Heroes.Select(h => h.Class.Name).Concat("None"));
        
        // TODO: Weight by rarity
        var selectedCards = cards
            .All
            .Where(x => x.LimitedToClass.IsMissingOr(c => partyClasses.Contains(c.Name)))
            .ToArray()
            .Shuffled()
            .Take(NumCards)
            .ToList();
        
        // TODO: Weight by rarity
        var selectedEquipment = equipment
            .All
            .Where(x => x.Classes.Any(c => partyClasses.Contains(c.Name)))
            .ToArray()
            .Shuffled()
            .Take(NumEquipment)
            .ToList();
        
        return new ShopSelection(selectedEquipment, selectedCards);
    }
}
