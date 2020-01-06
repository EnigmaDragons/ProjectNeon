public sealed class MemberStateChanged
{
    public Member Member { get; }
    
    public MemberStateChanged(Member m) => Member = m;
}
