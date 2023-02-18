public class OverrideCardDelay
{
    public TeamType Team { get; }
    public float Delay { get; }

    public OverrideCardDelay(TeamType team, float delay)
    {
        Team = team;
        Delay = delay;
    }
}