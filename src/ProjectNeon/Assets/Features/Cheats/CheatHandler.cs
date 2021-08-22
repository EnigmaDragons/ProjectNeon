using UnityEngine;

public class CheatHandler : OnMessage<GainRandomEquipment, CompleteAnyMapNode>
{
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
}