
public interface Equipment
{
    string Name { get; }
    string Description { get; }
    int Price { get; }
    Rarity Rarity { get; }
    string[] Classes { get; }
    EquipmentSlot Slot { get; }
    EquipmentStatModifier[] Modifiers { get; }
}