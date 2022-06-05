using System.Collections.Generic;
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
        conclusion.Set(true, "You won because you pressed the developer cheat buttons! Congratulations! This is an epic tale!", CurrentGameData.Data.Stats);
        navigator.NavigateToConclusionScene();
    }

    protected override void Execute(RespawnMap msg)
    {
        map5.CurrentChoices = new List<MapNode3>();
        Message.Publish(new RegenerateMapRequested());
    }
}