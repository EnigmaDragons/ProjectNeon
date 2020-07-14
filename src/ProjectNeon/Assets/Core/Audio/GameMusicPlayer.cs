using UnityEngine;

[CreateAssetMenu]
public sealed class GameMusicPlayer : ScriptableObject
{
    [SerializeField] private AudioSource musicSource;

    public void Init(AudioSource source)
    {
        StopMusicIfPlaying();
        musicSource = source;
    }

    public void InitIfNeeded(AudioSource source)
    {
        if (musicSource == null)
            Init(source);
    }

    public void FadeOutMusic(MonoBehaviour script, float duration)
    {
        script.StartCoroutine(musicSource.FadeOutAsync(duration));
    }
    
    public void PlaySelectedMusicOnce(AudioClip clipToPlay)
    {
        if (musicSource == null)
        {
            Debug.LogError($"nameof(musicSource) has not been initialized");
            return;
        }
        
        if (musicSource.isPlaying && musicSource.clip.name.Equals(clipToPlay.name))
            return;
        
        StopMusicIfPlaying();
        musicSource.clip = clipToPlay;
        musicSource.loop = false;
        musicSource.Play();
    }
    
    public void PlaySelectedMusicLooping(AudioClip clipToPlay)
    {
        if (musicSource == null)
        {
            Debug.LogError($"nameof(musicSource) has not been initialized");
            return;
        }
        
        if (musicSource.isPlaying && musicSource.clip.name.Equals(clipToPlay.name))
            return;
        
        StopMusicIfPlaying();
        musicSource.clip = clipToPlay;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void StopMusicIfPlaying()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();
    }
}
