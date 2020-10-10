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
        foreach (var adventure in library.UnlockedAdventures)
        {
            var adventureInstance = Instantiate(adventureDisplayPrefab, container.transform);
            adventureInstance.Init(adventure);
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