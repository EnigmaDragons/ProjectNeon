using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/SaveLoadSystem")]
public sealed class SaveLoadSystem : ScriptableObject
{
    [SerializeField] private StringReference versionNumber;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private Library library;

    public bool HasSavedGame => CurrentGameData.HasActiveGame;
    public void SaveCheckpoint() => CurrentGameData.Save();
    public void ClearCurrentSlot() => CurrentGameData.Clear();

    public void LoadSavedGame()
    {
        var saveData = CurrentGameData.Data;
        InitParty(saveData);
    }

    private void InitParty(GameData saveData)
    {
        var partyData = saveData.PartyData;
        var numHeroes = partyData.Heroes.Length;
        party.Initialized(
            library.HeroById(saveData.PartyData.Heroes[0].BaseHeroId),
            numHeroes > 1 ? library.HeroById(partyData.Heroes[1].BaseHeroId) : library.HeroById(0),
            numHeroes > 2 ? library.HeroById(partyData.Heroes[2].BaseHeroId) : library.HeroById(0));
    }
}
