using System.IO;
using UnityEngine;

public class InitCurrentGameAppDataSave : MonoBehaviour
{
    [SerializeField] private StringReference versionNumber;
    [SerializeField] private string saveFileName;

    private void Awake()
    {
        CurrentGameData.Init(versionNumber,
            new JsonFileStored<GameData>(Path.Combine(Application.persistentDataPath, saveFileName),
                () => new GameData {VersionNumber = versionNumber}));
        
        CurrentGameOptions.Init(versionNumber,             
            new JsonFileStored<PlayerOptionsData>(Path.Combine(Application.persistentDataPath, "options.json"),
                () => new PlayerOptionsData() {VersionNumber = versionNumber}));
        
        CurrentAcademyData.Init(new JsonFileStored<AcademyData>(Path.Combine(Application.persistentDataPath, "academy.json"), 
            () => new AcademyData()));
        
        var progressStored = new JsonFileStored<ProgressionData>(Path.Combine(Application.persistentDataPath, "progress.json"),
                () => new ProgressionData());
        if (!progressStored.FileExists())
            progressStored.Write(d => d);
        CurrentProgressionData.Init(progressStored, versionNumber);
    }
}
