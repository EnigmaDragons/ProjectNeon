using System;
using System.Collections.Generic;
using System.Linq;

public class PlayerState
{
    private IPlayerStats _playerBaseStats;
    private readonly List<ITemporalPlayerState> _mods = new List<ITemporalPlayerState>();
    
    public IPlayerStats CurrentStats => _playerBaseStats
        .Plus(_mods.Where(x => x.IsActive).Select(x => x.PlayerStats));

    public int CardDraws => CurrentStats.CardDraw();

    public PlayerState(int numCardCycles = 0)
    {
        _playerBaseStats = new PlayerStatAddends()
            .With(PlayerStatType.CardDraws, 6)
            .With(PlayerStatType.CardPlays, 3)
            .With(PlayerStatType.CardCycles, numCardCycles);
    }

    public void AddState(ITemporalPlayerState mod) => PublishAfter(() => _mods.Add(mod));

    public int BonusCredits { get; set; }

    public void OnTurnStart()
        => PublishAfter(() =>
        {
            _mods.ForEach(m => m.OnTurnStart());
            _mods.RemoveAll(m => !m.IsActive);
        });

    public void OnTurnEnd()
        => PublishAfter(() =>
        {
            _mods.ForEach(m => m.OnTurnEnd());
            _mods.RemoveAll(m => !m.IsActive);
        });

    private void PublishAfter(Action a)
    {
        a();
        Message.Publish(new PlayerStateChanged(this));
    }
}
