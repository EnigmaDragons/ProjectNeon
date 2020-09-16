using System.Collections.Generic;
using System.Linq;

public class PlayerState
{
    private readonly IPlayerStats _playerBaseStats;
    private readonly List<ITemporalPlayerState> _mods = new List<ITemporalPlayerState>();
    
    public IPlayerStats CurrentStats => _playerBaseStats
        .Plus(_mods.Where(x => x.IsActive).Select(x => x.PlayerStats));

    public PlayerState()
    {
        _playerBaseStats = new PlayerStatAddends()
            .With(PlayerStatType.CardDraws, 6)
            .With(PlayerStatType.CardPlays, 3)
            .With(PlayerStatType.CardCycles, 2);
    }

    public void AddState(ITemporalPlayerState mod) => _mods.Add(mod);

    public int BonusCredits { get; set; }

    public void OnTurnStart()
    {
        _mods.ForEach(m => m.OnTurnStart());
        _mods.RemoveAll(m => !m.IsActive);
    }
    
    public void OnTurnEnd()
    {
        _mods.ForEach(m => m.OnTurnEnd());
        _mods.RemoveAll(m => !m.IsActive);
    }
}
