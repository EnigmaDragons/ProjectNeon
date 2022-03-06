
public class StoryEventContext
{
    public int CurrentStage { get; }
    public RarityFactors RarityFactors { get; }
    public PartyAdventureState Party { get; }
    public EquipmentPool EquipmentPool { get; }
    public CurrentGameMap3 Map { get; }
    public AdventureProgressBase Adventure { get; }

    public StoryEventContext(int currentStage, RarityFactors rarityFactors, PartyAdventureState party, EquipmentPool equipmentPool, CurrentGameMap3 map, AdventureProgressBase adventure)
    {
        CurrentStage = currentStage;
        RarityFactors = rarityFactors;
        Party = party;
        EquipmentPool = equipmentPool;
        Map = map;
        Adventure = adventure;
    }
}
