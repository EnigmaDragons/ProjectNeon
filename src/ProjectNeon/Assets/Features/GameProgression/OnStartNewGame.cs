
using UnityEngine;

public class OnStartNewGame : OnMessage<StartNewGame>
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private Hero[] heroes;
    [SerializeField] private Party party;
    
    protected override void Execute(StartNewGame msg)
    {
        party.Initialized(heroes[0], heroes[1], heroes[2]);
        navigator.NavigateToGameScene();
    }
}
