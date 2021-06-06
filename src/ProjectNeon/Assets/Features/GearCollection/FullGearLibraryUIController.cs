using UnityEngine;

public class FullGearLibraryUIController : OnMessage<ToggleGearLibrary>
{
    [SerializeField] private GameObject library;

    private void Awake() => library.SetActive(false);

    protected override void Execute(ToggleGearLibrary msg) => library.SetActive(!library.activeSelf);
}