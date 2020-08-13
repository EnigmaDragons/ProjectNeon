
public class MemberStateChanged
{
    public MemberState State { get; }
    public MemberStateSnapshot BeforeState { get; }

    public MemberStateChanged(MemberStateSnapshot before, MemberState after) => State = after;
}
