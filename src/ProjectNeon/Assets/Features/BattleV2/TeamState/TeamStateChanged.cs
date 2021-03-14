public class TeamStateChanged
{
    public TeamState Team { get; }

    public TeamStateChanged(TeamState team) => Team = team;
}