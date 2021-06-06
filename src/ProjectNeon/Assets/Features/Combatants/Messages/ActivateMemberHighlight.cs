
public class ActivateMemberHighlight
{
    public int MemberId { get; }
    public MemberHighlightType HighlightType { get; }
    public bool ExclusiveForType { get; }

    public ActivateMemberHighlight(int memberId, MemberHighlightType highlightType, bool exclusiveForType)
    {
        MemberId = memberId;
        HighlightType = highlightType;
        ExclusiveForType = exclusiveForType;
    }
}