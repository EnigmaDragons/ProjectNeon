
public class AvoidanceContext
{
    public static readonly AvoidanceContext None = new AvoidanceContext(AvoidanceType.NotSpecified, new Member[0]);
    
    public AvoidanceType Type { get; }
    public Member[] Members { get; }

    public AvoidanceContext(AvoidanceType a, Member[] members)
    {
        Type = a;
        Members = members;
    }
}
