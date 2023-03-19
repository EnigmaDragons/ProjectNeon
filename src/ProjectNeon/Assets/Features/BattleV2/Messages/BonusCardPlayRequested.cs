public class BonusCardPlayRequested
{
    public Member Member { get; }
    public BonusCardDetails Details { get; }

    public BonusCardPlayRequested(Member member, BonusCardDetails details)
    {
        Member = member;
        Details = details;
    }
}