
using UnityEngine;

public class GameStarter : OnMessage<StartNewGame, ContinueCurrentGame>
{
    [SerializeField] private Navigator _navigator;
    [SerializeField] private SaveLoadSystem io;
    
    protected override void Execute(StartNewGame msg)
    {
        io.ClearCurrentSlot();
        _navigator.NavigateToAdventureSelection();
    }

    protected override void Execute(ContinueCurrentGame msg)
    {
        if (io.HasSavedGame)
        {
            io.LoadSavedGame();
            _navigator.NavigateToAdventureSelection();
        }
    }
}
