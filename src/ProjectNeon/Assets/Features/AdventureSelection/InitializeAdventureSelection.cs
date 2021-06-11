using UnityEngine;

public class InitializeAdventureSelection : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private AdventureDisplayPresenter adventureDisplayPrefab;
    [SerializeField] private Library library;
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private AdventureProgress2 adventureProgress2;
    [SerializeField] private Navigator navigator;

    private void Start()
    {
        for (var i = 0; i < library.UnlockedAdventures.Length; i++)
        {
            var adventure = library.UnlockedAdventures[i];
            var adventureInstance = Instantiate(adventureDisplayPrefab, container.transform);
            var currentIndex = i;
            adventureInstance.Init(adventure, () => BeginAdventure(currentIndex));
        }
    }

    public void BeginAdventure(int index)
    {
        if (library.UnlockedAdventures.Length > index)
        {
            var adventure = library.UnlockedAdventures[index];
            currentAdventure.Adventure = adventure;
            if (currentAdventure.Adventure.IsV2)
                adventureProgress2.Init();
            CurrentGameData.Write(s =>
            {
                s.IsInitialized = true;
                s.Phase = CurrentGamePhase.SelectedAdventure;
                s.AdventureProgress = new GameAdventureProgressData
                {
                    AdventureId = adventure.Id
                };
                return s;
            });
            navigator.NavigateToSquadSelection();
        }
    }
}