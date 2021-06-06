
public class DeactivateMemberHighlight
{
    public int MemberId { get; }
    public MemberHighlightType HighlightType { get; }

    public DeactivateMemberHighlight(int memberId, MemberHighlightType highlightType)
    {
        MemberId = memberId;
        HighlightType = highlightType;
    }
}