using UnityEngine;

public class QuickStartNewGame : OnMessage<StartNewGame>
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private BaseHero[] heroes;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private Adventure adventure;
    [SerializeField] private AdventureProgress progress;
    
    protected override void Execute(StartNewGame msg)
    {
        progress.Init(adventure);
        party.Initialized(heroes[0], heroes[1], heroes[2]);
        navigator.NavigateToGameScene();
    }
}
