using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/SaveLoadSystem")]
public sealed class SaveLoadSystem : ScriptableObject
{
    [SerializeField] private StringReference versionNumber;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private AdventureProgress2 adventure;
    [SerializeField] private AdventureProgressV4 adventurev4;
    [SerializeField] private AdventureProgressV5 adventurev5;
    [SerializeField] private CurrentAdventureProgress adventureProgress;
    [SerializeField] private Library library;
    [SerializeField] private CurrentGameMap3 map;
    [SerializeField] private CurrentMapSegmentV5 mapV5;
    [SerializeField] private AllMaps maps;
    [SerializeField] private CorpClinicProvider clinics;
    [SerializeField] private AllStaticGlobalEffects globalEffects;
    [SerializeField] private DeterminedNodeInfo determinedNodeInfo;
    [SerializeField] private DraftState draftState;

    public bool HasSavedGame => CurrentGameData.HasActiveGame;
    public void SaveCheckpoint() => SaveCurrentGame();
    public void ClearCurrentSlot() => CurrentGameData.Clear();

    private void SaveCurrentGame()
    {
        CurrentGameData.WriteIfInitialized(s =>
        {
            s.AdventureProgress = adventureProgress.AdventureProgress.GetData();
            s.PartyData = party.GetData();
            s.GameMap = adventureProgress.AdventureProgress.AdventureType.GetMapData(map, mapV5);
            s.Stats = s.Stats.WithAdditionalElapsedTime(RunTimer.ConsumeElapsedTime());
            s.DeterminedData = determinedNodeInfo.GetData();
            s.DraftData = draftState.GetData();
            return s;
        });
    }
    
    public void SaveDecks()
    {
        CurrentGameData.WriteIfInitialized(s =>
        {
            s.PartyData = party.GetData();
            return s;
        });
    }

    public void SaveDeterminations()
    {
        CurrentGameData.WriteIfInitialized(s =>
        {
            s.DeterminedData = determinedNodeInfo.GetData();
            return s;
        });
    }

    public CurrentGamePhase LoadSavedGame()
    {
        var saveData = CurrentGameData.Data;
        var loadedSuccessfully = true;
        if (!string.IsNullOrWhiteSpace(saveData.RunId))
            AllMetrics.SetRunId(saveData.RunId);
        if (loadedSuccessfully && saveData.FinishedPhase(CurrentGamePhase.SelectedAdventure))
            loadedSuccessfully = InitAdventure(saveData.AdventureProgress);
        if (loadedSuccessfully && saveData.FinishedPhase(CurrentGamePhase.SelectedSquad))
            loadedSuccessfully = InitParty(saveData.PartyData);
        if (loadedSuccessfully)
            loadedSuccessfully = determinedNodeInfo.SetData(saveData.DeterminedData);
        if (loadedSuccessfully && saveData.FinishedPhase(CurrentGamePhase.SelectedAdventure))
            loadedSuccessfully = InitMap(saveData.GameMap);
        if (loadedSuccessfully)
            loadedSuccessfully = draftState.Init(saveData.PartyData.Heroes.Length, saveData.DraftData);
        if (!loadedSuccessfully)
            return CurrentGamePhase.LoadError;

        return saveData.Phase;
    }

    private bool InitAdventure(GameAdventureProgressData d)
    {
        var selectedAdventure = library.GetAdventureById(d.AdventureId);
        if (selectedAdventure.IsMissing)
            return LoadFailedReason($"Unknown Adventure {d.AdventureId}");
        
        if (d.Type == GameAdventureProgressType.V2)
        {
            adventureProgress.AdventureProgress = adventure;
            return adventure.InitAdventure(d, selectedAdventure.Value);
        }

        if (d.Type == GameAdventureProgressType.V4)
        {
            adventureProgress.AdventureProgress = adventurev4;
            return adventurev4.InitAdventure(d, selectedAdventure.Value);
        }

        if (d.Type == GameAdventureProgressType.V5)
        {
            adventureProgress.AdventureProgress = adventurev5;   
            return adventurev5.InitAdventure(d, selectedAdventure.Value);
        }
        
        return LoadFailedReason($"Unknown Adventure Type {d.Type}");
    }

    private bool InitParty(GamePartyData partyData)
    {
        var numHeroes = partyData.Heroes.Length;
        if (numHeroes == 0)
            return true;
        
        var maybeCards = partyData.CardIds.Select(id => library.GetCardById(id)).ToArray();
        if (maybeCards.Any(c => c.IsMissing))
            return LoadFailedReason("Missing Cards");

        var maybeEquipments = partyData.Equipment.Select(e => library.GetEquipment(e)).ToArray();
        if (maybeEquipments.Any(c => c.IsMissing))
            return LoadFailedReason("Missing Equipments");

        party.InitFromSave(
            library.HeroById(partyData.Heroes[0].BaseHeroId),
            numHeroes > 1 ? library.HeroById(partyData.Heroes[1].BaseHeroId) : library.HeroById(0),
            numHeroes > 2 ? library.HeroById(partyData.Heroes[2].BaseHeroId) : library.HeroById(0),
            partyData.Credits,
            partyData.ClinicVouchers,
            maybeCards.Select(c => c.Value).ToArray(),
            maybeEquipments.Select(e => e.Value).ToArray());
        
        var deckMaybeCards = partyData.Heroes.Select(h => h.Deck.CardIds.Select(id => library.GetCardById(id).Value).ToList()).ToArray();
        if (!deckMaybeCards.Any())
            return LoadFailedReason("Missing Cards From Decks");
        party.UpdateDecks(deckMaybeCards);

        // Don't blow up the load over a missing blessing. Just don't grant it.
        var heroesById = partyData.Blessings
            .SelectMany(b => b.TargetHeroIds)
            .Distinct()
            .Select(h => library.HeroById(h))
            .ToDictionary(h => h.Id, h => h);
        foreach (var blessingSaveData in partyData.Blessings)
        {
            var maybeBlessingData = clinics.GetBlessingByName(blessingSaveData.Name);
            var data = blessingSaveData;
            maybeBlessingData.IfPresent(b =>
            {
                var targetHeroes = data.TargetHeroIds.Select(id => heroesById[id]).ToArray();
                party.AddBlessing(new Blessing { Name = b.Name, Effect = b.Effect, Targets = targetHeroes, Duration = data.Duration });
            });
        }
        
        party.SetCorpCostModifier(partyData.CorpCostModifiers);

        for (var i = 0; i < numHeroes; i++)
        {
            var hero = party.Heroes[i];
            var heroSaveData = partyData.Heroes[i];
            
            hero.SetLevels(heroSaveData.Levels);
            hero.SetHealth(heroSaveData.Health);
            hero.SetPrimaryStat(heroSaveData.PrimaryStat);

            var maybeBasicCard = library.GetCardById(heroSaveData.BasicCardId);
            if (!maybeBasicCard.IsPresent)
                return LoadFailedReason("Unknown Basic Card");
            hero.SetBasic(maybeBasicCard.Value);

            foreach (var equipmentIdName in heroSaveData.EquipmentIdNames)
            {
                Log.Info($"Load Equipment - {equipmentIdName}");
                if (equipmentIdName.Name.Equals("Implant"))
                    continue;
                
                var maybeEquipment = party.Equipment.Available.Where(x => x.Id == equipmentIdName.Id && x.Name.Equals(equipmentIdName.Name)).FirstAsMaybe();
                if (!maybeEquipment.IsPresent)
                    return LoadFailedReason($"Cannot find Hero's Equipped {equipmentIdName.Name} in party equipment");
                if (maybeEquipment.Value.Slot != EquipmentSlot.Permanent)
                    party.EquipTo(maybeEquipment.Value, hero);
            }

            foreach (var implant in heroSaveData.Implants)
                hero.ApplyImplant(implant.GeneratedEquipment);

            foreach (var equipment in party.GlobalEquipment)
                hero.ApplyPermanent(equipment);
            
            hero.AddToStats(new StatAddends(heroSaveData.Stats.ToDictionary(x => x.Stat, x => x.Value)));
            
            var generatedLevelUpOptionId = -2;
            var draftLevelUpOptionId = -3;
            foreach (var optionId in hero.Levels.SelectedLevelUpOptionIds.Where(id => id != generatedLevelUpOptionId && id != draftLevelUpOptionId))
            {
                var maybePerk = library.GetLevelUpPerkById(optionId);
                if (!maybePerk.IsPresent)
                    return LoadFailedReason("Select Level Up Perk not found");
                maybePerk.Value.Apply(hero);
            }
        }

        return true;
    }

    private bool InitMap(GameMapData mapData)
    {
        if (mapData.Type == GameMapDataType.V5)
            return InitMapV5(mapData);
        else if (mapData.Type == GameMapDataType.V3)
            return InitMapV3(mapData);
        else
            return true;
    }
    
    private bool InitMapV5(GameMapData mapData)
    {
        var selectedMap = maps.GetMapById(mapData.GameMapId);
        if (selectedMap.IsMissing)
            return LoadFailedReason($"Unknown Map {mapData.GameMapId}");
        mapV5.CurrentMap = selectedMap.Value;
        mapV5.CurrentNode = mapData.CurrentNode;
        mapV5.PreviousPosition = mapData.CurrentPosition;
        mapV5.DestinationPosition = mapData.CurrentPosition;
        mapV5.CurrentChoices = mapData.CurrentChoices.ToList();
        mapV5.CurrentNodeRngSeed = ConsumableRngSeed.Init(mapData.CurrentNodeRngSeed);
        mapV5.IncludeCurrentNodeInSaveData = mapData.IncludeCurrentNodeInSave;
        return true;
    }
    
    private bool InitMapV3(GameMapData mapData)
    {
        var selectedMap = maps.GetMapById(mapData.GameMapId);
        if (selectedMap.IsMissing)
            return LoadFailedReason($"Unknown Map {mapData.GameMapId}");
        map.CurrentMap = selectedMap.Value;
        map.CurrentNode = mapData.CurrentNode;
        map.CompletedNodes = mapData.CompletedNodes.ToList();
        map.PreviousPosition = mapData.CurrentPosition;
        map.DestinationPosition = mapData.CurrentPosition;
        map.CurrentChoices = mapData.CurrentChoices.ToList();
        map.HasCompletedEventEnRoute = mapData.HasCompletedEventEnRoute;
        map.CurrentNodeRngSeed = mapData.CurrentNodeRngSeed;
        map.HeatAdjustments = mapData.HeatAdjustments;
        return true;
    }
    
    private bool LoadFailedReason(string reason)
    {
        if (!reason.Equals("Unknown Adventure 8"))
            Log.Error($"Load Failed - {reason}");
        return false;
    }
}
