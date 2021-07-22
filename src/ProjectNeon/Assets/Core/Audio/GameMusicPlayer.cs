using UnityEngine;

[CreateAssetMenu]
public sealed class GameMusicPlayer : ScriptableObject
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private bool showDebugLogs = true;
    
    public void Init(AudioSource source)
    {
        DebugLog("Init");
        StopMusicIfPlaying();
        musicSource = source;
    }

    public void InitIfNeeded(AudioSource source)
    {
        if (musicSource == null)
            Init(source);
    }

    public void FadeOutMusic(MonoBehaviour script, float duration) => script.StartCoroutine(musicSource.FadeOutAsync(duration));
    public void PlaySelectedMusicOnce(AudioClip clipToPlay) => Play(clipToPlay, false);
    public void PlaySelectedMusicLooping(AudioClip clipToPlay) => Play(clipToPlay, true);

    private void Play(AudioClip c, bool shouldLoop)
    {
        if (c == null)
        {
            Log.Warn("Asked to play null AudioClip");
            return;
        }
        
        if (musicSource == null)
        {
            Log.Error($"nameof(musicSource) has not been initialized");
            return;
        }

        var currentClipName = musicSource.clip?.name ?? "None";
        DebugLog($"Playing Clip: {currentClipName} | New Clip: {c.name}");
        if (musicSource.isPlaying && currentClipName.Equals(c.name))
            return;
        
        StopMusicIfPlaying();
        musicSource.clip = c;
        musicSource.loop = true;
        DebugLog($"Play Music: {musicSource.clip?.name ?? "None"} | New Clip: {c.name}");
        musicSource.Play();
    }

    private void StopMusicIfPlaying()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();
    }

    private void DebugLog(string action)
    {        
        if (showDebugLogs)
            Log.Info($"Game Music Player - {action} - AudioSource Enabled {musicSource != null && musicSource.enabled} - Audio Game Object Enabled {musicSource != null && musicSource.gameObject.activeSelf}");
    }
}
