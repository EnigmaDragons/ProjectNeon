
public class CustomStatusIcon
{
    public string Tooltip { get; }
    public string IconName { get; }
    public int Number { get; }
    public int OriginatorId { get; }
    public TemporalStateTracker StateTracker { get; }

    public CustomStatusIcon(string tooltip, string iconName, int number, TemporalStateMetadata temporalMetadata)
    {
        Tooltip = tooltip;
        IconName = iconName;
        Number = number;
        OriginatorId = temporalMetadata.OriginatorId;
        StateTracker = new TemporalStateTracker(temporalMetadata);
    }

    public string DisplayNumber => Number > 0 ? Number.ToString() : "";
}
