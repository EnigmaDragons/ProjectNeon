using System.IO;
using Features.GameProgression;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/SaveLoadSystem")]
public sealed class SaveLoadSystem : ScriptableObject
{
    [SerializeField] private StringReference versionNumber;
    [SerializeField] private PartyAdventureState party;

    private string _currentSlot = "defaultSave.json";

    public void SaveCheckpoint()
    {
        var data = SaveStateConverters.Create(versionNumber.Value, party);
        var jsonIo = new JsonFileStored<SaveState>(Path.Combine(Application.persistentDataPath, _currentSlot), () => new SaveState());
        jsonIo.Write(_ => data);
    }

    public void ClearCurrentSlot()
    {
        var jsonIo = new JsonFileStored<SaveState>(Path.Combine(Application.persistentDataPath, _currentSlot), () => new SaveState());
        jsonIo.Write(_ => new SaveState());
    }
}
