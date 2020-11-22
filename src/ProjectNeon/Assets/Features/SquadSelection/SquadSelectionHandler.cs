
using UnityEngine;

public class SquadSelectionHandler : OnMessage<ConfirmSquadSelection>
{
    [SerializeField] private HeroPool pool;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private Navigator navigator;
    [SerializeField] private CurrentAdventure current;
    
    protected override void Execute(ConfirmSquadSelection msg)
    {        
        var heroes = pool.SelectedHeroes;
        party.Initialized(heroes[0], heroes[1], heroes[2]);
        if (current.Adventure.IsV2)
            navigator.NavigateToGameScene2();
        else
            navigator.NavigateToGameScene();
    }
}
