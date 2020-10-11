using UnityEngine;

public class InitializeAdventureSelection : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private AdventureDisplayPresenter adventureDisplayPrefab;
    [SerializeField] private Library library;
    [SerializeField] private AdventureProgress adventureProgress;
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
            adventureProgress.Init(adventure);
            navigator.NavigateToSquadSelection();
            
        }
    }
}