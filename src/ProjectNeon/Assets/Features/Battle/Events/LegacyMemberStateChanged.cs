public sealed class LegacyMemberStateChanged
{
    public Member Member { get; }
    
    public LegacyMemberStateChanged(Member m) => Member = m;
}
