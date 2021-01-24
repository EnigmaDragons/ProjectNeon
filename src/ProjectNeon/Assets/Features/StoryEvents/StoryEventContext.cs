
public class StoryEventContext
{
    public PartyAdventureState Party { get; }
    public EquipmentPool EquipmentPool { get; }

    public StoryEventContext(PartyAdventureState party, EquipmentPool equipmentPool)
    {
        Party = party;
        EquipmentPool = equipmentPool;
    }
}
