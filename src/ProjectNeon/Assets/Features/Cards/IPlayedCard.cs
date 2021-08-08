public interface IPlayedCard
{
    int PlayedCardId { get; }
    Member Member { get; }
    Card Card { get; }
    Target[] Targets { get; }
    ResourceQuantity Spent { get; }
    ResourceQuantity Gained { get; }
    bool IsSingleUse { get; }
    bool IsTransient { get; }

    void Perform(BattleStateSnapshot beforeCard);
}

public static class IPlayedCardExtensions
{
    public static int MemberId(this IPlayedCard c) => c.Member.Id;
    public static int PrimaryTargetId(this IPlayedCard c) => c.Targets[0].Members[0].Id;
}

public sealed class PlayedCardSnapshot
{
    public MemberSnapshot Member { get; }
    public bool WasDiscarded { get; }
    public CardTypeData Card { get; }
    public Target[] Targets { get; }
    public ResourceQuantity Spent { get; }
    public ResourceQuantity Gained { get; }
    public bool WasTransient { get; }

    public PlayedCardSnapshot(IPlayedCard p)
    {
        Member = p.Member.GetSnapshot();
        WasDiscarded = !p.Card.IsActive;
        Card = p.Card.Type;
        Targets = p.Targets;
        Spent = p.Spent;
        Gained = p.Gained;
        WasTransient = p.IsTransient;
    }

}
