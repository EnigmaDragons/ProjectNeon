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
