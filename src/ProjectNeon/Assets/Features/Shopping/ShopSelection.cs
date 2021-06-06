using System.Collections.Generic;

public class ShopSelection
{
    public List<Equipment> Equipment { get; }
    public List<CardTypeData> Cards { get; }

    public ShopSelection(List<Equipment> equipment, List<CardTypeData> cards)
    {
        Equipment = equipment;
        Cards = cards;
    }
}
