using UnityEngine;

[CreateAssetMenu]
public sealed class UiSfxPlayer : ScriptableObject
{
    [SerializeField] private AudioSource source;
    [SerializeField] private bool showDebugLogs = true;

    public void Init(AudioSource src)
    {
        source = src;
        DebugLog("Init");
    }

    public void InitIfNeeded(AudioSource src)
    {
        if (source == null)
            Init(src);
    }

    public void Play(AudioClipVolume c) => Play(c.clip, c.volume);
    public void Play(AudioClip c, float volume = 1f)
    {
        DebugLog($"Play {c.name}");
        if (c == null)
            Log.Error("Tried to play a Null UI Sound Effect");
        else if (source == null)
            Log.Error("UI SFX Player is not initialized");
        else
            source.PlayOneShot(c, volume);
    }
    
    private void DebugLog(string action)
    {        
        if (showDebugLogs)
            Log.Info($"UI SFX Player - {action} - AudioSource Enabled {source != null && source.enabled} - Audio Game Object Enabled {source != null && source.gameObject.activeSelf}");
    }
}
