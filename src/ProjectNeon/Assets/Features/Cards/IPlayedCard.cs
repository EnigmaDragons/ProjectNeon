public interface IPlayedCard
{
    int PlayedCardId { get; }
    Member Member { get; }
    Card Card { get; }
    Target[] Targets { get; }
    ResourceCalculations Calculations { get; }
    ResourceQuantity Spent { get; }
    bool IsSingleUse { get; }
    bool IsTransient { get; }
    bool RetargetingAllowed { get; }

    void Perform(BattleStateSnapshot beforeCard);
}

public static class IPlayedCardExtensions
{
    public static int MemberId(this IPlayedCard c) => c.Member.Id;
    public static int PrimaryTargetId(this IPlayedCard c) => c.Targets[0].Members[0].Id;
    public static IPlayedCard Retargeted(this IPlayedCard c, Target[] targets) => new PlayedCardV2(c.Member, targets, c.Card, c.IsTransient, c.RetargetingAllowed, c.Calculations);
}

public sealed class PlayedCardSnapshot
{
    public MemberSnapshot Member { get; }
    public bool WasDiscarded { get; }
    public CardTypeData Card { get; }
    public Target[] Targets { get; }
    public ResourceQuantity Spent { get; }
    public bool WasTransient { get; }

    public PlayedCardSnapshot(IPlayedCard p)
    {
        Member = p.Member.GetSnapshot();
        WasDiscarded = !p.Card.IsActive;
        Card = p.Card.Type;
        Targets = p.Targets;
        Spent = p.Spent;
        WasTransient = p.IsTransient;
    }

}
