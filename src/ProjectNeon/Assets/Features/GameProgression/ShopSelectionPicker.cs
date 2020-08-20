
using System.Linq;

public class ShopSelectionPicker
{
    private static readonly int NumCards = 4;
    private static readonly int NumEquipment = 2;
    
    public ShopSelection GenerateSelection(ShopCardPool cards, EquipmentPool equipment, PartyAdventureState party)
    {
        // TODO: Only select cards that can be equipped by the party
        // TODO: Weight by rarity
        var selectedCards = cards.All.ToArray().Shuffled().Take(NumCards).ToList();
        // TODO: Only select equipment that can be equipped by the party
        // TODO: Weight by rarity
        var selectedEquipment = equipment.All.ToArray().Shuffled().Take(NumEquipment).ToList();
        return new ShopSelection(selectedEquipment, selectedCards);
    }
}
