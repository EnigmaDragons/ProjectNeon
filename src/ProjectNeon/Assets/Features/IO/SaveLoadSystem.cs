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
        {
            Log.Error($"Unknown Adventure {adventureProgress.AdventureId}");
            return false;
        }
        adventure.Init(selectedAdventure.Value);
        return true;
    }

    private bool InitParty(GamePartyData partyData)
    {
        var numHeroes = partyData.Heroes.Length;
        var maybeCards = partyData.CardIds.Select(id => library.GetCardById(id));
        if (maybeCards.Any(c => c.IsMissing))
        {
            Log.Error($"Load Failed - Missing Some Cards");
            return false;
        }
        
        party.InitFromSave(
            library.HeroById(partyData.Heroes[0].BaseHeroId),
            numHeroes > 1 ? library.HeroById(partyData.Heroes[1].BaseHeroId) : library.HeroById(0),
            numHeroes > 2 ? library.HeroById(partyData.Heroes[2].BaseHeroId) : library.HeroById(0),
            partyData.Credits,
            maybeCards.Select(c => c.Value).ToArray());

        var deckMaybeCards = partyData.Heroes.Select(h => h.Deck.CardIds.Select(id => library.GetCardById(id)).ToList());
        if (deckMaybeCards.Any(d => d.Any(c => c.IsMissing)))
        {
            Log.Error($"Load Failed - Missing Some Cards from Decks");
            return false;
        }
        party.UpdateDecks(deckMaybeCards.Select(d => d.Select(c => c.Value).ToList()).ToArray());
        
        for (var i = 0; i < numHeroes; i++)
        {
            var maybeBasicCard = library.GetCardById(partyData.Heroes[i].BasicCardId);
            if (!maybeBasicCard.IsPresent)
                Log.Error($"Load Failed - Unknown Basic Card");
            party.Heroes[i].SetBasic(maybeBasicCard.Value);
        }
        
        return true;
    }
}
