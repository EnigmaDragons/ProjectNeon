﻿using System;
using UnityEngine;

[Serializable]
public class GameData
{
    public string VersionNumber = "Not Initialized";
    public string RunId = Guid.NewGuid().ToString();
    public bool IsInitialized = false;
    public CurrentGamePhase Phase = CurrentGamePhase.NotStarted;
    public GameTutorialData TutorialData = new GameTutorialData();
    public GameAdventureProgressData AdventureProgress = new GameAdventureProgressData();
    public GamePartyData PartyData = new GamePartyData();
    public GameMapData GameMap = new GameMapData();
    public RunStats Stats = new RunStats();

    public bool FinishedPhase(CurrentGamePhase phase) => (int)Phase >= (int)phase;
}

[Serializable]
public class GameAdventureProgressData
{
    public int AdventureId = -1;
    public GameAdventureProgressType Type = GameAdventureProgressType.Unknown;
    public int CurrentChapterIndex = -1;
    public int CurrentSegmentIndex = -1;
    public int[] CurrentChapterFinishedHeatUpEvents = new int[0];
    public string[] FinishedStoryEvents = new string[0];
    public bool PlayerReadMapPrompt = false;
    public int[] ActiveGlobalEffectIds = new int[0];
    public int RngSeed = Rng.NewSeed();
}

public enum GameAdventureProgressType
{
    Unknown = 0,
    V2 = 2,
    V4 = 4
}

[Serializable]
public class GameTutorialData
{
    public bool ShouldShowTutorials = false;
    public string[] CompletedTutorialNames = Array.Empty<string>();
}

[Serializable]
public class GamePartyData
{
    public int Credits = 0;
    public GameHeroData[] Heroes = Array.Empty<GameHeroData>();
    public int[] CardIds = Array.Empty<int>();
    public GameEquipmentData[] Equipment = Array.Empty<GameEquipmentData>();
    public BlessingSaveData[] Blessings = Array.Empty<BlessingSaveData>();
    public CorpCostModifier[] CorpCostModifiers;
}

[Serializable]
public class GameHeroData
{
    public int BaseHeroId = -1;
    public int BasicCardId = -1;
    public HeroLevels Levels = new HeroLevels();
    public HeroHealth Health = new HeroHealth();
    public StatAddendData[] Stats = new StatAddendData[0];
    public GameDeckData Deck = new GameDeckData();
    public GameEquipmentIdName[] EquipmentIdNames = Array.Empty<GameEquipmentIdName>();
    public GameEquipmentData[] Implants = Array.Empty<GameEquipmentData>();
    public Maybe<StatType> PrimaryStat = Maybe<StatType>.Missing();
}

[Serializable]
public class BlessingSaveData
{
    public string Name = "";
    public int[] TargetHeroIds = Array.Empty<int>();
}

[Serializable]
public class GameDeckData
{
    public int[] CardIds = Array.Empty<int>();
}

[Serializable]
public class GameEquipmentIdName
{
    public int Id = -1;
    public string Name = "";
}

[Serializable]
public class GameEquipmentData
{
    public GameEquipmentDataType Type = GameEquipmentDataType.None;
    public int StaticEquipmentId = -1;
    public InMemoryEquipment GeneratedEquipment = new InMemoryEquipment();
}

[Serializable]
public class GameMapData
{
    public int GameMapId;
    public Maybe<MapNode3> CurrentNode;
    public MapNode3[] CompletedNodes;
    public Vector2 CurrentPosition;
    public MapNode3[] CurrentChoices;
    public bool HasCompletedEventEnRoute;
    public int CurrentNodeRngSeed;
    public int HeatAdjustments;
}

[Serializable]
public class StatAddendData
{
    public string Stat;
    public float Value;
}

[Serializable]
public class RunStats
{
    public int TimeElapsedSeconds;
    public int TotalDamageDealt;
    public int TotalDamageReceived;
    public int TotalHpDamagedReceived;
    public int TotalCardsPlayed;
    public int TotalTurnsPlayed;
}

public enum GameEquipmentDataType
{
    None = 0,
    StaticEquipmentId = 1,
    GeneratedEquipment = 2
}

