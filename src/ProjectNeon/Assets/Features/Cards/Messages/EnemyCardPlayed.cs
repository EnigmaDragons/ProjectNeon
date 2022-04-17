
public class EnemyCardPlayed
{
    public IPlayedCard PlayedCard { get; }

    public EnemyCardPlayed(IPlayedCard c)
    {
        PlayedCard = c;
    }
}
