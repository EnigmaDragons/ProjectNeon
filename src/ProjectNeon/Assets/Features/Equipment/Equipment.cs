
public interface Equipment
{
    string Name { get; }
    string Description { get; }
    Rarity Rarity { get; }
    CharacterClass[] Classes { get; }
    EquipmentSlot Slot { get; }
}