
public sealed class MixerVolumeChanged
{
    public string ChannelName { get; set;  }

    public MixerVolumeChanged(string name) => ChannelName = name;
}
