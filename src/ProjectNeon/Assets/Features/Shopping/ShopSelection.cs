using System.Collections.Generic;

public class ShopSelection
{
    public List<StaticEquipment> Equipment { get; }
    public List<CardTypeData> Cards { get; }

    public ShopSelection(List<StaticEquipment> equipment, List<CardTypeData> cards)
    {
        Equipment = equipment;
        Cards = cards;
    }
}
