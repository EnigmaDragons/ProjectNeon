using System;

[Serializable]
public class GameData
{
    public string VersionNumber = "Not Initialized";
    public bool IsInitialized = false;
    public CurrentGamePhase Phase = CurrentGamePhase.NotStarted;
    public GamePartyData PartyData = new GamePartyData();
    public GameAdventureProgressData AdventureProgress = new GameAdventureProgressData();
}

[Serializable]
public class GameAdventureProgressData
{
    public int AdventureId = -1;
}

[Serializable]
public class GamePartyData
{
    public int Credits = 0;
    public GameHeroData[] Heroes = Array.Empty<GameHeroData>();
    public int[] CardIds = Array.Empty<int>();
    public GameEquipmentData[] Equipment = Array.Empty<GameEquipmentData>();
}

[Serializable]
public class GameHeroData
{
    public int BaseHeroId = -1;
    public int BasicCardId = -1;
    public GameDeckData Deck = new GameDeckData();
}

[Serializable]
public class GameDeckData
{
    public int[] CardIds = Array.Empty<int>();
}


[Serializable]
public class GameEquipmentData
{
    public GameEquipmentDataType Type = GameEquipmentDataType.None;
    public int StaticEquipmentId = -1;
    public InMemoryEquipment GeneratedEquipment = new InMemoryEquipment();
}

public enum GameEquipmentDataType
{
    None = 0,
    StaticEquipmentId = 1,
    GeneratedEquipment = 2
}

