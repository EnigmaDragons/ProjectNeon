
using UnityEngine;

public class StartGameWithSquadSelect : OnMessage<StartNewGame>
{
    [SerializeField] private Navigator _navigator;
    [SerializeField] private SaveLoadSystem io;
    
    protected override void Execute(StartNewGame msg)
    {
        io.ClearCurrentSlot();
        _navigator.NavigateToAdventureSelection();
    }
}
