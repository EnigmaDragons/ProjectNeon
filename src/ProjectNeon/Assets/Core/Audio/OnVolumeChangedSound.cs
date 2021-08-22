using UnityEngine;
using UnityEngine.Audio;

public class OnVolumeChangedSound : OnMessage<MixerVolumeChanged>
{
    [SerializeField] private AudioSource player;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioClipVolume[] sounds;

    private bool _triggered;
    private MixerVolumeChanged _lastChange;
    
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
        var clip = sounds.FirstOrMaybe(s => s.Name.Equals(_lastChange.ChannelName));
        clip.IfPresent(c => player.PlayOneShot(c.clip));
    }
}
