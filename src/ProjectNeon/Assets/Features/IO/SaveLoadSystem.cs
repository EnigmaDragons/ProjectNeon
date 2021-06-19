using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/SaveLoadSystem")]
public sealed class SaveLoadSystem : ScriptableObject
{
    [SerializeField] private StringReference versionNumber;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private AdventureProgress2 adventure;
    [SerializeField] private Library library;

    public bool HasSavedGame => CurrentGameData.HasActiveGame;
    public void SaveCheckpoint() => SaveCurrentGame();
    public void ClearCurrentSlot() => CurrentGameData.Clear();

    private void SaveCurrentGame()
    {
        CurrentGameData.Write(s =>
        {
            s.IsInitialized = true;
            s.AdventureProgress = new GameAdventureProgressData
            {
                AdventureId = adventure.CurrentAdventureId
            };
            s.PartyData = party.GetData();
            return s;
        });
    }

    public CurrentGamePhase LoadSavedGame()
    {
        var saveData = CurrentGameData.Data;
        var loadedSuccessfully = true;
        if (loadedSuccessfully && (int) saveData.Phase >= (int) CurrentGamePhase.SelectedAdventure)
            loadedSuccessfully = InitAdventure(saveData.AdventureProgress);
        if (loadedSuccessfully && (int) saveData.Phase >= (int) CurrentGamePhase.SelectedSquad)
            loadedSuccessfully = InitParty(saveData.PartyData);
        if (!loadedSuccessfully) 
            Log.Info("Unable to Load Game");

        return saveData.Phase;
    }

    private bool InitAdventure(GameAdventureProgressData adventureProgress)
    {
        var selectedAdventure = library.GetAdventureById(adventureProgress.AdventureId);
        if (selectedAdventure.IsMissing)
            return LoadFailedReason($"Unknown Adventure {adventureProgress.AdventureId}");
        adventure.Init(selectedAdventure.Value);
        return true;
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
        
        for (var i = 0; i < numHeroes; i++)
        {
            var hero = party.Heroes[i];
            var heroSaveData = partyData.Heroes[i];
            
            hero.SetLevels(heroSaveData.Levels);
            hero.SetHealth(heroSaveData.Health);
            
            var maybeBasicCard = library.GetCardById(heroSaveData.BasicCardId);
            if (!maybeBasicCard.IsPresent)
                return LoadFailedReason("Unknown Basic Card");
            hero.SetBasic(maybeBasicCard.Value);

            foreach (var equipmentIdName in heroSaveData.EquipmentIdNames)
            {
                var maybeEquipment = party.Equipment.Available.Where(x => x.Id == equipmentIdName.Id && x.Name.Equals(equipmentIdName.Name)).FirstAsMaybe();
                if (!maybeEquipment.IsPresent)
                    return LoadFailedReason($"Cannot find Hero's Equipped {equipmentIdName.Name} in party equipment");
                party.EquipTo(maybeEquipment.Value, hero);
            }
        }

        return true;
    }
    
    private bool LoadFailedReason(string reason)
    {
        Log.Error($"Load Failed - {reason}");
        return false;
    }
}
