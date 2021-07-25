public class GeneralMetric
{
    public string EventType { get; }
    public string Event { get; }

    public GeneralMetric(string eventType, string e)
    {
        EventType = eventType;
        Event = e;
    }
}
