
public class ShowStoryEventResolution
{
    public string Story { get; }
    public int OutcomeValue { get; }
    public bool IsGoodOutcome => OutcomeValue > 0;
    public bool IsBadOutcome => OutcomeValue < 0;

    public ShowStoryEventResolution(string story, int outcomeValue)
    {
        Story = story;
        OutcomeValue = outcomeValue;
    }
}
