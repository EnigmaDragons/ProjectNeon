using UnityEngine;

public class CheatHandler : OnMessage<GainRandomEquipment, CompleteAnyMapNode, WinGameRequested>
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private AdventureConclusionState conclusion;
    [SerializeField] private AdventureProgress2 adventure;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private EquipmentPool equipments;
    [SerializeField] private CurrentGameMap3 map;
        
    protected override void Execute(GainRandomEquipment msg)
    {
        var picker = adventure.CreateLootPicker(party);
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
        conclusion.Set(true, "You won because you pressed the developer cheat buttons! Congratulations! This is an epic tale!");
        navigator.NavigateToConclusionScene();
    }
}