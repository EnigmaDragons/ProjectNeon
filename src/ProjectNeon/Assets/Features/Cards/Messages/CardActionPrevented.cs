
public class CardActionPrevented
{
    public Member Source { get; }
    public TemporalStatType ToDecrement { get; }

    public CardActionPrevented(Member source, TemporalStatType toDecrement)
    {
        Source = source;
        ToDecrement = toDecrement;
    }
}
