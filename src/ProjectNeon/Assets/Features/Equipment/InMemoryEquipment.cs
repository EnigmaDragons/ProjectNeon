
public class InMemoryEquipment : Equipment
{
    public string Name { set; get; }
    public string Description { set; get; }
    public int Price { set; get; }
    public Rarity Rarity { set; get; }
    public string[] Classes { set; get; } = new string[1] { CharacterClass.All };
    public EquipmentSlot Slot { set; get; }
    public EquipmentStatModifier[] Modifiers { get; set; }

    public Equipment Initialized(CharacterClass characterClass)
    {
        Classes = new [] {characterClass.Name };
        return this;
    }
}
