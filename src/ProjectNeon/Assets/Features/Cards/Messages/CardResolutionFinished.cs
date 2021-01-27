public class CardResolutionFinished
{
    public bool CardWasInstant { get; }
    public int MemberId { get; }

    public CardResolutionFinished(int memberId, bool cardWasInstant)
    {
        MemberId = memberId;
        CardWasInstant = cardWasInstant;
    }
}
