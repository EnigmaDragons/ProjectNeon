using System.Collections.Generic;
using UnityEngine;

public class PlayerUiSoundGuy : MonoBehaviour
{
    [SerializeField] private UiSfxPlayer sfx;
    [SerializeField] private AudioClipVolume onSelect;
    [SerializeField] private AudioClipVolume onCancel;
    [SerializeField] private float repeatCooldown = 0.4f;

    private Dictionary<string, float> _lastPlayed = new Dictionary<string, float>();
    
    private void OnEnable()
    {
        _lastPlayed.Clear();
        Message.Subscribe<PlayerCardSelected>(_ => Play(onSelect), this);
        Message.Subscribe<PlayerCardCanceled>(_ => Play(onCancel), this);
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }

    private void Play(AudioClipVolume a)
    {
        var now = Time.timeSinceLevelLoad;
        if (_lastPlayed.TryGetValue(a.clip.name, out var lastPlayed) && now - lastPlayed < repeatCooldown)
            return;

        _lastPlayed[a.clip.name] = now;
        sfx.Play(a);
    }
}
