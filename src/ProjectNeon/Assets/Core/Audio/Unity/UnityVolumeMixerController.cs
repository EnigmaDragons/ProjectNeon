using UnityEngine;
using UnityEngine.Audio;

public class UnityVolumeMixerController : OnMessage<MixerVolumeChanged>
{
    [SerializeField] private AudioMixer mixer;
    
    protected override void Execute(MixerVolumeChanged msg)
    {
        var mixerVolume = Mathf.Log10(msg.Volume) * 20;
        mixer.SetFloat(msg.ChannelName, msg.Volume <= 0 ? -80 : mixerVolume);
    }
}