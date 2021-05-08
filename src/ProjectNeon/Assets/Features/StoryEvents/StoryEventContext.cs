
public class StoryEventContext
{
    public int CurrentStage { get; }
    public RarityFactors RarityFactors { get; }
    public PartyAdventureState Party { get; }
    public EquipmentPool EquipmentPool { get; }

    public StoryEventContext(int currentStage, RarityFactors rarityFactors, PartyAdventureState party, EquipmentPool equipmentPool)
    {
        CurrentStage = currentStage;
        RarityFactors = rarityFactors;
        Party = party;
        EquipmentPool = equipmentPool;
    }
}
