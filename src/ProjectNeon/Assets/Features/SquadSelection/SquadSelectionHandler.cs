using System.Linq;
using UnityEngine;

public class SquadSelectionHandler : OnMessage<ConfirmSquadSelection>
{
    [SerializeField] private HeroPool pool;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private Navigator navigator;
    [SerializeField] private CurrentAdventureProgress adventure;
    [SerializeField] private CurrentAdventure currentAdventure;

    private bool _confirmed;
    
    protected override void Execute(ConfirmSquadSelection msg)
    {
        if (_confirmed)
            return;
        
        _confirmed = true;
        Log.Info("Confirmed Squad Selection");
        var heroes = pool.SelectedHeroes;
        // party.Initialized(heroes[0], heroes[1], heroes[2]);
        // AllMetrics.PublishSelectedParty(adventure.AdventureProgress.AdventureName, heroes.Select(h => h.Name).ToArray());
        // CurrentGameData.Write(s =>
        // {
        //     s.IsInitialized = true;
        //     s.Phase = CurrentGamePhase.SelectedSquad;
        //     s.PartyData = party.GetData();
        //     return s;
        // });
        Message.Publish(new StartAdventureV5Requested(currentAdventure.Adventure, pool.SelectedHeroes, adventure.HasActiveAdventure ? adventure.AdventureProgress.Difficulty : Maybe<Difficulty>.Missing()));
    }
}
