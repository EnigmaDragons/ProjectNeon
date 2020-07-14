using UnityEngine;
using UnityEngine.Audio;

public class OnVolumeChangedSound : OnMessage<MixerVolumeChanged>
{
    [SerializeField] private AudioClip sound;
    [SerializeField] private AudioSource player;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioClip[] narratorSounds;

    private IndexSelector<AudioClip> _narratorSounds;
    private bool _triggered;
    private MixerVolumeChanged _lastChange;

    private void Awake() => _narratorSounds = new IndexSelector<AudioClip>(narratorSounds);
    
    protected override void Execute(MixerVolumeChanged msg)
    {
        _triggered = true;
        _lastChange = msg;
    }

    private void Update()
    {
        if (!_triggered || Input.GetMouseButton(0)) return;

        _triggered = false;
        var mixerGroups = mixer.FindMatchingGroups(_lastChange.ChannelName);
        if (mixerGroups.Length <= 0) return;
        
        player.outputAudioMixerGroup =  mixerGroups[0];
        var clip = _lastChange.ChannelName.Equals("NarratorVolume") ? _narratorSounds.MoveNextWithoutLooping() : sound;
        player.PlayOneShot(clip);
    }
}
