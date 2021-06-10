using System.IO;
using UnityEngine;

public class InitCurrentGameAppDataSave : MonoBehaviour
{
    [SerializeField] private string saveFileName;

    private void Awake() => CurrentGameData.Init(
        new JsonFileStored<GameData>(Path.Combine(Application.persistentDataPath, saveFileName), () => new GameData()));
}
