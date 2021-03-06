﻿using System.IO;
using UnityEngine;

public class InitCurrentGameAppDataSave : MonoBehaviour
{
    [SerializeField] private StringReference versionNumber;
    [SerializeField] private string saveFileName;

    private void Awake() => CurrentGameData.Init(versionNumber,
        new JsonFileStored<GameData>(Path.Combine(Application.persistentDataPath, saveFileName), () => new GameData { VersionNumber = versionNumber }));
}
