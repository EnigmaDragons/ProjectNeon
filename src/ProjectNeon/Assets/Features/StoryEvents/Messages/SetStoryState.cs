public class SetStoryState
{
    public string State { get; }
    public bool Value { get; }

    public SetStoryState(string state, bool value)
    {
        State = state;
        Value = value;
    }
}