using UnityEngine;

public class FullLibraryUIController : OnMessage<ToggleCardLibrary>
{
    [SerializeField] private GameObject library;

    private void Awake() => library.SetActive(false);

    protected override void Execute(ToggleCardLibrary msg) => library.SetActive(!library.activeSelf);
}
