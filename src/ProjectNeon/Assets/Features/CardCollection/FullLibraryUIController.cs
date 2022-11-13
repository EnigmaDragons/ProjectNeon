using UnityEngine;

public class FullLibraryUIController : OnMessage<ToggleCardLibrary, ToggleEnemyCardLibrary>
{
    [SerializeField] private GameObject library;
    [SerializeField] private GameObject enemyLibrary;

    private void Awake()
    {
        library.SetActive(false);
        if (enemyLibrary != null)
            enemyLibrary.SetActive(false);
    }

    protected override void Execute(ToggleCardLibrary msg) => library.SetActive(!library.activeSelf);
    protected override void Execute(ToggleEnemyCardLibrary msg)
    {
        if (enemyLibrary != null)
            enemyLibrary.SetActive(!enemyLibrary.activeSelf);
    }
}
