public interface IPlayedCard
{
    Member Member { get; }
    Card Card { get; }
    ResourcesSpent Spent { get; }

    void Perform();
}