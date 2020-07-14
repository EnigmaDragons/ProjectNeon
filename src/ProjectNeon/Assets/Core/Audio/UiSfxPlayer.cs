using UnityEngine;

[CreateAssetMenu]
public sealed class UiSfxPlayer : ScriptableObject
{
    [SerializeField] private AudioSource source;

    public void Init(AudioSource src) => source = src;
    public void InitIfNeeded(AudioSource src) => source.IfNull(() => Init(src));
    public void Play(AudioClipVolume c) => Play(c.clip, c.volume);
    public void Play(AudioClip c, float volume = 1f)
    {
        if (source != null)
            source.PlayOneShot(c, volume);
    }
}
