public interface IPlayedCard
{
    Member Member { get; }
    Card Card { get; }
    void Perform();
}