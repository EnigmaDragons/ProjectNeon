
public class StoryEventContext
{
    public RarityFactors RarityFactors { get; }
    public PartyAdventureState Party { get; }
    public EquipmentPool EquipmentPool { get; }

    public StoryEventContext(RarityFactors rarityFactors, PartyAdventureState party, EquipmentPool equipmentPool)
    {
        RarityFactors = rarityFactors;
        Party = party;
        EquipmentPool = equipmentPool;
    }
}
