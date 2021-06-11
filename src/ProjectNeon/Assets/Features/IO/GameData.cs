using System;

[Serializable]
public class GameData
{
    public string VersionNumber = "Not Initialized";
    public bool IsInitialized = false;
    public GamePartyData PartyData = new GamePartyData();
    public GameAdventureProgressData AdventureProgress = new GameAdventureProgressData();
}

[Serializable]
public class GameAdventureProgressData
{
}

[Serializable]
public class GamePartyData
{
    public int Credits = 0;
    public int[] CardIds = Array.Empty<int>();
    public GameHeroData[] Heroes = Array.Empty<GameHeroData>();
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
