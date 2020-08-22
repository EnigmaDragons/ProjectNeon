
public interface Equipment
{
    string Name { get; }
    string Description { get; }
    int Price { get; }
    Rarity Rarity { get; }
    CharacterClass[] Classes { get; }
    EquipmentSlot Slot { get; }
}