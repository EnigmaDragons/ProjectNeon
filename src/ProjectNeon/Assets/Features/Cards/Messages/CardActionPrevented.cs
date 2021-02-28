
public class CardActionPrevented
{
    public Member Source { get; }
    public AvoidanceContext Avoid { get; }
    public TemporalStatType ToDecrement { get; }

    public CardActionPrevented(Member source, AvoidanceContext avoid, TemporalStatType toDecrement)
    {
        Source = source;
        Avoid = avoid;
        ToDecrement = toDecrement;
    }
}
