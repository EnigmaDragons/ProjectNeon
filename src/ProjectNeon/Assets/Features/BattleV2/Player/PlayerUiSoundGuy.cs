using System.Collections.Generic;
using UnityEngine;

public class PlayerUiSoundGuy : MonoBehaviour
{
    [SerializeField] private UiSfxPlayer sfx;
    [SerializeField] private AudioClipVolume onSelect;
    [SerializeField] private AudioClipVolume onCancel;
    [SerializeField] private AudioClipVolume onCardDrawn;
    [SerializeField] private AudioClipVolume onShuffled;
    [SerializeField] private float repeatCooldown = 0.4f;

    private readonly Dictionary<string, float> _lastPlayed = new Dictionary<string, float>();
    
    private void OnEnable()
    {
        _lastPlayed.Clear();
        Message.Subscribe<PlayerCardSelected>(_ => Play(onSelect), this);
        Message.Subscribe<PlayerCardCanceled>(_ => Play(onCancel), this);
        Message.Subscribe<PlayerCardDrawn>(_ => Play(onCardDrawn, false), this);
        Message.Subscribe<PlayerDeckShuffled>(_ => Play(onShuffled), this);
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }

    private void Play(AudioClipVolume a, bool requiresCooldown = true)
    {
        var now = Time.timeSinceLevelLoad;
        if (requiresCooldown && _lastPlayed.TryGetValue(a.clip.name, out var lastPlayed) && now - lastPlayed < repeatCooldown)
            return;
        
        Log.Info($"Playing Sound {a.clip.name}");
        _lastPlayed[a.clip.name] = now;
        sfx.Play(a);
    }
}
