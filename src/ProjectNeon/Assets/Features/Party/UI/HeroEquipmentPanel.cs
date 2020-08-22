
using UnityEngine;

public class HeroEquipmentPanel : MonoBehaviour
{
    [SerializeField] private HeroEquipmentItemPresenter weaponSlot;
    [SerializeField] private HeroEquipmentItemPresenter armorSlot;
    [SerializeField] private HeroEquipmentItemPresenter augment1Slot;
    [SerializeField] private HeroEquipmentItemPresenter augment2Slot;
    [SerializeField] private HeroEquipmentItemPresenter augment3Slot;

    public HeroEquipmentPanel Initialized(Hero h)
    {
        var equipment = h.Equipment;
        equipment.Weapon.IfPresent(e => weaponSlot.Initialized(e));
        equipment.Armor.IfPresent(e => armorSlot.Initialized(e));
        var augments = equipment.Augments;
        if (augments.Length > 0)
            augment1Slot.Initialized(augments[0]);
        if (augments.Length > 1)
            augment2Slot.Initialized(augments[1]);
        if (augments.Length > 2)
            augment3Slot.Initialized(augments[2]);
        return this;
    }
}
