using UnityEngine;
using UnityEngine.Audio;

public class UnityVolumeMixerController : OnMessage<MixerVolumeChanged>
{
    [SerializeField] private AudioMixer mixer;
    
    protected override void Execute(MixerVolumeChanged msg)
    {
        mixer.SetFloat(msg.ChannelName, msg.Volume);
    }
}