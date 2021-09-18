
public class MixerVolumeChangeRequested
{
    public string ChannelName { get; }
    public float Volume { get; }

    public MixerVolumeChangeRequested(string name, float volume)
    {
        ChannelName = name;
        Volume = volume;
    }
}