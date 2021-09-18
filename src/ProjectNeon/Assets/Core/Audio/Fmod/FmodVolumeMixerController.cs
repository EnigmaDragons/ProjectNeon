using UnityEngine;

public class FmodVolumeMixerController : OnMessage<MixerVolumeChanged>
{
    [SerializeField] private FmodGameAudioManager fmod;
    
    protected override void Execute(MixerVolumeChanged msg)
    {
        if (msg.ChannelName.Equals("MusicVolume"))
            fmod.SetMusicVolumeLevel(msg.Volume);
        else if (msg.ChannelName.Equals("SoundVolume"))
            fmod.SetGameSfxVolumeLevel(msg.Volume);
        else if (msg.ChannelName.Equals("UiVolume"))
            fmod.SetUiSfxVolumeLevel(msg.Volume);
    }
}
