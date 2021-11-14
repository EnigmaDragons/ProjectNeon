using System.Linq;
using Features.GameProgression;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/SaveLoadSystem")]
public sealed class SaveLoadSystem : ScriptableObject
{
    [SerializeField] private StringReference versionNumber;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private AdventureProgress2 adventure;
    [SerializeField] private AdventureProgressV4 adventurev4;
    [SerializeField] private CurrentAdventureProgress adventureProgress;
    [SerializeField] private Library library;
    [SerializeField] private CurrentGameMap3 map;
    [SerializeField] private AllMaps maps;
    [SerializeField] private CorpClinicProvider clinics;
    [SerializeField] private AllStaticGlobalEffects globalEffects;

    public bool HasSavedGame => CurrentGameData.HasActiveGame;
    public void SaveCheckpoint() => SaveCurrentGame();
    public void ClearCurrentSlot() => CurrentGameData.Clear();

    public void SetShouldShowTutorials(bool shouldShow) => CurrentGameData.Write(s =>
    {
        s.TutorialData = new GameTutorialData
        {
            ShouldShowTutorials = shouldShow,
            CompletedTutorialNames = s.TutorialData.CompletedTutorialNames
        };
        return s;
    });
    
    private void SaveCurrentGame()
    {
        CurrentGameData.Write(s =>
        {
            s.IsInitialized = true;
            s.AdventureProgress = adventureProgress.AdventureProgress.GetData();
            s.PartyData = party.GetData();
            s.GameMap = map.GetData();
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
        if (loadedSuccessfully && saveData.FinishedPhase(CurrentGamePhase.SelectedAdventure) 
                               && saveData.AdventureProgress.Type != GameAdventureProgressType.V4)
            loadedSuccessfully = InitMap(saveData.GameMap);
        if (!loadedSuccessfully)
            return CurrentGamePhase.LoadError;

        return saveData.Phase;
    }

    private bool InitAdventure(GameAdventureProgressData adventureProgress)
    {
        var selectedAdventure = library.GetAdventureById(adventureProgress.AdventureId);
        if (selectedAdventure.IsMissing)
            return LoadFailedReason($"Unknown Adventure {adventureProgress.AdventureId}");
        if (adventureProgress.Type == GameAdventureProgressType.V2)
            return adventure.InitAdventure(adventureProgress, selectedAdventure.Value);
        if (adventureProgress.Type == GameAdventureProgressType.V4)
            return adventurev4.InitAdventure(adventureProgress, selectedAdventure.Value);
        return LoadFailedReason($"Unknown Adventure Type {adventureProgress.Type}");
    }

    private bool InitParty(GamePartyData partyData)
    {
        var numHeroes = partyData.Heroes.Length;
        var maybeCards = partyData.CardIds.Select(id => library.GetCardById(id));
        if (maybeCards.Any(c => c.IsMissing))
            return LoadFailedReason("Missing Cards");

        var maybeEquipments = partyData.Equipment.Select(e => library.GetEquipment(e));
        if (maybeEquipments.Any(c => c.IsMissing))
            return LoadFailedReason("Missing Equipments");
        
        party.InitFromSave(
            library.HeroById(partyData.Heroes[0].BaseHeroId),
            numHeroes > 1 ? library.HeroById(partyData.Heroes[1].BaseHeroId) : library.HeroById(0),
            numHeroes > 2 ? library.HeroById(partyData.Heroes[2].BaseHeroId) : library.HeroById(0),
            partyData.Credits,
            maybeCards.Select(c => c.Value).ToArray(),
            maybeEquipments.Select(e => e.Value).ToArray());
        
        var deckMaybeCards = partyData.Heroes.Select(h => h.Deck.CardIds.Select(id => library.GetCardById(id)).ToList());
        if (deckMaybeCards.Any(d => d.Any(c => c.IsMissing)))
            return LoadFailedReason("Missing Cards From Decks");
        party.UpdateDecks(deckMaybeCards.Select(d => d.Select(c => c.Value).ToList()).ToArray());

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
                var targetHeroes = data.TargetHeroIds.Select(id => heroesById[id]).Cast<HeroCharacter>().ToArray();
                party.AddBlessing(new Blessing { Name = b.Name, Effect = b.Effect, Targets = targetHeroes });
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
                if (equipmentIdName.Name.Equals("Implant"))
                    continue;
                
                var maybeEquipment = party.Equipment.Available.Where(x => x.Id == equipmentIdName.Id && x.Name.Equals(equipmentIdName.Name)).FirstAsMaybe();
                if (!maybeEquipment.IsPresent)
                    return LoadFailedReason($"Cannot find Hero's Equipped {equipmentIdName.Name} in party equipment");
                if (maybeEquipment.Value.Slot != EquipmentSlot.Permanent)
                    party.EquipTo(maybeEquipment.Value, hero);
            }

            foreach (var implant in heroSaveData.Implants)
                hero.ApplyPermanent(implant.GeneratedEquipment);

            foreach (var optionId in hero.Levels.SelectedLevelUpOptionIds)
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
        Log.Error($"Load Failed - {reason}");
        return false;
    }
}
