using UnityEngine;

public class SquadSelectionHandler : OnMessage<ConfirmSquadSelection>
{
    [SerializeField] private HeroPool pool;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private Navigator navigator;

    protected override void Execute(ConfirmSquadSelection msg)
    {        
        var heroes = pool.SelectedHeroes;
        party.Initialized(heroes[0], heroes[1], heroes[2]);
        CurrentGameData.Write(s =>
        {
            s.IsInitialized = true;
            s.Phase = CurrentGamePhase.SelectedSquad;
            s.PartyData = party.GetData();
            return s;
        });
        navigator.NavigateToGameScene();
    }
}
