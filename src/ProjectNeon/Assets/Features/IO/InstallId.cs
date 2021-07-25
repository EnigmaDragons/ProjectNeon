using System;
using System.IO;
using UnityEngine;

public static class InstallId
{
    private static readonly JsonFileStored<InstallIdRecord> StoredInstallId =
        new JsonFileStored<InstallIdRecord>(Path.Combine(Application.persistentDataPath, ".installId"), () => new InstallIdRecord());

    public static string Get()
    {
        if (!StoredInstallId.FileExists())
            StoredInstallId.Write(_ => new InstallIdRecord());
        return StoredInstallId.Get().id;
    }

    [Serializable]
    private class InstallIdRecord
    {
        public string id = Guid.NewGuid().ToString();
    }
}