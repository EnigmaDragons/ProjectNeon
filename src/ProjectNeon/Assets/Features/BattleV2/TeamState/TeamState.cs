using System;
using System.Collections.Generic;

public class TeamState
{
    private readonly Dictionary<string, BattleCounter> _counters =
        new Dictionary<string, BattleCounter>(StringComparer.InvariantCultureIgnoreCase);
    
    public TeamType Team { get; }
    public int Shields => _counters[TeamStatType.Shield.ToString()].Amount;
    
    public TeamState(TeamType team)
    {
        Team = team;
        _counters[TeamStatType.Shield.ToString()] =
            new BattleCounter(TeamStatType.Shield, 0, () => 100);
    }

    public void Adjust(TeamStatType type, int amount) => PublishAfter(() => _counters[type.ToString()].ChangeBy(amount));

    private void PublishAfter(Action a)
    {
        a();
        Message.Publish(new TeamStateChanged(this));
    }
}