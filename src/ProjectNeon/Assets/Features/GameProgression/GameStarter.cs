
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
            var phase = io.LoadSavedGame();
            if (phase == CurrentGamePhase.NotStarted)
                _navigator.NavigateToAdventureSelection();
            else if (phase == CurrentGamePhase.SelectedAdventure)
                _navigator.NavigateToSquadSelection();
            else if (phase == CurrentGamePhase.SelectedSquad)
                _navigator.NavigateToGameScene();
        }
    }
}
