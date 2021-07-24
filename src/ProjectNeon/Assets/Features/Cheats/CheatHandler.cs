using UnityEngine;

public class CheatHandler : OnMessage<GainRandomEquipment>
{
    [SerializeField] private AdventureProgress2 adventure;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private EquipmentPool equipments; 
        
    protected override void Execute(GainRandomEquipment msg)
    {
        var picker = adventure.CreateLootPicker(party);
        party.Add(picker.PickEquipments(equipments, 1));
    }
}