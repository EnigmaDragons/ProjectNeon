using UnityEngine;

public static class PlayerAudioPrefs
{
    public static string Music => "MusicVolume";
    public static string Sfx => "SoundVolume";
    public static string Ui => "UiVolume";
    
    public static float GetVolumeLevel(string busName)
    {
        return PlayerPrefs.GetFloat(busName, 0.5f);
    }
    
    public static void SetVolumeLevel(string busName, float amount)
    {
        PlayerPrefs.SetFloat(busName, amount);
        Message.Publish(new MixerVolumeChanged(busName, amount));
    }
}