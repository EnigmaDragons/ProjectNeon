public class TriggerCutsceneEvent
{
    public string EventName { get; }
    public float DurationBeforeContinue { get; }

    public TriggerCutsceneEvent(string eventName, float durationBeforeContinue)
    {
        EventName = eventName;
        DurationBeforeContinue = durationBeforeContinue;
    }
}
