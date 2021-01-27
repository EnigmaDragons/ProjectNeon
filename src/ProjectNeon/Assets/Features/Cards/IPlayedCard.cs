public interface IPlayedCard
{
    Member Member { get; }
    Card Card { get; }
    Target[] Targets { get; }
    ResourceQuantity Spent { get; }
    ResourceQuantity Gained { get; }
    bool IsTransient { get; }

    void Perform(BattleStateSnapshot beforeCard);
}

public static class IPlayedCardExtensions
{
    public static bool IsInstant(this IPlayedCard c) => c.Card.TimingType == CardTimingType.Instant;
    public static bool IsHasty(this IPlayedCard c) => c.Card.TimingType == CardTimingType.Hasty;
}
