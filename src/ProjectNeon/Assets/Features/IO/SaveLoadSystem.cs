using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/SaveLoadSystem")]
public sealed class SaveLoadSystem : ScriptableObject
{
    [SerializeField] private StringReference versionNumber;
    [SerializeField] private PartyAdventureState party;
    
    public void SaveCheckpoint() => CurrentGameData.Save();

    public void ClearCurrentSlot() => CurrentGameData.Clear();
}
