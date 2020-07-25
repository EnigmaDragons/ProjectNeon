
public class BattleFinished
{
    public TeamType Winner { get; }

    public BattleFinished(TeamType winner) => Winner = winner;
}
