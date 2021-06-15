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
    public void SaveCheckpoint() => CurrentGameData.Save();
    public void ClearCurrentSlot() => CurrentGameData.Clear();

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
            Log.Error($"Missing Some Cards");
            return false;
        }
        
        party.InitFromSave(
            library.HeroById(partyData.Heroes[0].BaseHeroId),
            numHeroes > 1 ? library.HeroById(partyData.Heroes[1].BaseHeroId) : library.HeroById(0),
            numHeroes > 2 ? library.HeroById(partyData.Heroes[2].BaseHeroId) : library.HeroById(0),
            partyData.Credits,
            maybeCards.Select(c => c.Value).ToArray());
        return true;
    }
}
