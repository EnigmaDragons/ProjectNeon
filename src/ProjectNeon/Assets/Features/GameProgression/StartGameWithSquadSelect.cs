
using UnityEngine;

public class StartGameWithSquadSelect : OnMessage<StartNewGame>
{
    [SerializeField] private Navigator _navigator;
    
    protected override void Execute(StartNewGame msg)
    {
        _navigator.NavigateToSquadSelection();
    }
}
