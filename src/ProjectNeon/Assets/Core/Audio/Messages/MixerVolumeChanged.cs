
public sealed class MixerVolumeChanged
{
    public string ChannelName { get; }
    public float Volume { get; }

    public MixerVolumeChanged(string name, float volume)
    {
        ChannelName = name;
        Volume = volume;
    }
}
