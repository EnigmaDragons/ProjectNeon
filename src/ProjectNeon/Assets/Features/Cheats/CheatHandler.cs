using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheatHandler : OnMessage<GainRandomEquipment, CompleteAnyMapNode, WinGameRequested, RespawnMap>
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private AdventureConclusionState conclusion;
    [SerializeField] private CurrentAdventureProgress adventure;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private EquipmentPool equipments;
    [SerializeField] private CurrentGameMap3 map;
    [SerializeField] private CurrentMapSegmentV5 map5;
    [SerializeField] private CurrentBoss boss;
    [SerializeField] private CurrentAdventure currentAdventure;
        
    protected override void Execute(GainRandomEquipment msg)
    {
        var picker = adventure.AdventureProgress.CreateLootPicker(party);
        party.Add(picker.PickEquipments(equipments, 1));
    }

    protected override void Execute(CompleteAnyMapNode msg)
    {
        map.CurrentNode = map.CurrentChoices.Random();
        map.CompleteCurrentNode();
        Message.Publish(new NodeFinished());
    }

    protected override void Execute(WinGameRequested msg)
    {
        CurrentProgressionData.RecordCompletedAdventure(adventure.AdventureProgress.AdventureId, adventure.AdventureProgress.Difficulty.id, boss.Boss.id, party.Heroes.Select(h => h.Character.Id).ToArray());
        conclusion.RecordFinishedGameAndCleanUp(true, currentAdventure.Adventure.VictoryConclusionTerm, CurrentGameData.Data.Stats, party.Heroes);
        navigator.NavigateToConclusionScene();
    }

    protected override void Execute(RespawnMap msg)
    {
        map5.CurrentChoices = new List<MapNode3>();
        Message.Publish(new RegenerateMapRequested());
    }
}