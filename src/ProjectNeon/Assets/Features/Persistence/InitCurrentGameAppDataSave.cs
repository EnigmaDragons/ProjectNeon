using UnityEngine;

public class InitCurrentGameAppDataSave : MonoBehaviour
{
    [SerializeField] private string saveFileName;

    private void Awake() => CurrentGameData.Init(new JsonFileStored<GameData>(saveFileName, () => new GameData()));
}
