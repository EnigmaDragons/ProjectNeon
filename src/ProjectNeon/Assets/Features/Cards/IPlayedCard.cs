public interface IPlayedCard
{
    Member Member { get; }
    Card Card { get; }
    ResourceQuantity Spent { get; }
    ResourceQuantity Gained { get; }

    void Perform(BattleStateSnapshot beforeCard);
}
