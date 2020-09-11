using System.Linq;
using TMPro;
using UnityEngine;

public sealed class EquipmentSummaryPanel : OnMessage<PartyAdventureStateChanged>
{
    [SerializeField] private PartyAdventureState state;
    [SerializeField] private TextMeshProUGUI weaponSummaryLabel;
    [SerializeField] private TextMeshProUGUI armorSummaryLabel;
    [SerializeField] private TextMeshProUGUI augmentSummaryLabel;

    protected override void AfterEnable() => UpdateUi();
    
    protected override void Execute(PartyAdventureStateChanged msg) => UpdateUi();

    private void UpdateUi()
    {
        weaponSummaryLabel.text = Summary(EquipmentSlot.Weapon);
        armorSummaryLabel.text = Summary(EquipmentSlot.Armor);
        augmentSummaryLabel.text = Summary(EquipmentSlot.Augmentation);
    }

    private string Summary(EquipmentSlot s)
    {
        var total = state.Equipment.All.Count(x => x.Slot == s);
        var available = state.Equipment.Available.Count(x => x.Slot == s);
        return $"{available}/{total}";
    }
}
