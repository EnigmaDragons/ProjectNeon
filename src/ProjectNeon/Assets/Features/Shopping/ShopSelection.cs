using System.Collections.Generic;

public class ShopSelection
{
    public List<StaticEquipment> Equipment { get; }
    public List<CardType> Cards { get; }

    public ShopSelection(List<StaticEquipment> equipment, List<CardType> cards)
    {
        Equipment = equipment;
        Cards = cards;
    }
}
