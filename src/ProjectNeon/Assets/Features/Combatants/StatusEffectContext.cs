
public class StatusEffectContext
{
    public Member Source { get; }
    public Member Member { get; }

    public StatusEffectContext(Member s, Member m)
    {
        Source = s;
        Member = m;
    } 
}
