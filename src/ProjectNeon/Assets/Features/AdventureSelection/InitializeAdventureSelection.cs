using UnityEngine;

public class InitializeAdventureSelection : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private AdventureDisplayPresenter adventureDisplayPrefab;
    [SerializeField] private Library library;

    private void Start()
    {
        foreach (var adventure in library.UnlockedAdventures)
        {
            var adventureInstance = Instantiate(adventureDisplayPrefab, container.transform);
            adventureInstance.Init(adventure);
        }
    }
}