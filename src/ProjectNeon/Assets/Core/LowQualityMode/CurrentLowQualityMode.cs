using System;
using UnityEngine;

public class CurrentLowQualityMode
{
    private const string PlayerPrefKey = "LowQualityMode";
    
    public static bool IsEnabled { get; private set; }

    public static void Set(bool isEnabled) => PublishAfter(() => IsEnabled = isEnabled);
    public static void Enable() => PublishAfter(() => IsEnabled = true);
    public static void Disable() => PublishAfter(() => IsEnabled = false);
    public static void Toggle() => PublishAfter(() => IsEnabled = !IsEnabled);
    
    private static void PublishAfter(Action action) 
    {
        action();
        PlayerPrefs.SetInt(PlayerPrefKey, IsEnabled ? 1 : 0);
        PlayerPrefs.Save();
        Message.Publish(new LowQualityModeChanged(IsEnabled));
    }

    public static void InitFromPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey(PlayerPrefKey))
        {
#if UNITY_STANDALONE_LINUX
            PlayerPrefs.SetInt(PlayerPrefKey, 0);
#else
            PlayerPrefs.SetInt(PlayerPrefKey, IsEnabled ? 1 : 0);
#endif
            PlayerPrefs.Save();
        }
        IsEnabled = PlayerPrefs.GetInt(PlayerPrefKey) == 1;
    }
}
