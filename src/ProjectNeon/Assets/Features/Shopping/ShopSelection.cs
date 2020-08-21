using System.Collections.Generic;

public class ShopSelection
{
    public List<Equipment> Equipment { get; }
    public List<CardType> Cards { get; }

    public ShopSelection(List<Equipment> equipment, List<CardType> cards)
    {
        Equipment = equipment;
        Cards = cards;
    }
}
