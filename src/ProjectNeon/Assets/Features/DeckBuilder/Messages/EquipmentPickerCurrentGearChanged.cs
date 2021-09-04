using UnityEngine;

public class EquipmentPickerCurrentGearChanged
{
    public Transform UiSource { get; }
    public bool IsEquipped { get; }

    public EquipmentPickerCurrentGearChanged(Transform uiSource, bool isEquipped)
    {
        UiSource = uiSource;
        IsEquipped = isEquipped;
    }
}