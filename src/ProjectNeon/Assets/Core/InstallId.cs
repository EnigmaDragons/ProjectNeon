using System;
using UnityEngine;

public sealed class InstallId
{
    public string Id { get; }

    private InstallId(string id) => Id = id;

    public static InstallId FromPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey(nameof(InstallId)))
        {
            PlayerPrefs.SetString(nameof(InstallId), Guid.NewGuid().ToString());
            PlayerPrefs.Save();
        }

        return new InstallId(PlayerPrefs.GetString(nameof(InstallId)));
    }
}
