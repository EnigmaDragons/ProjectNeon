
using UnityEngine;

public class HeroEquipmentPanel : MonoBehaviour
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private HeroEquipmentItemPresenter weaponSlot;
    [SerializeField] private HeroEquipmentItemPresenter armorSlot;
    [SerializeField] private HeroEquipmentItemPresenter augment1Slot;
    [SerializeField] private HeroEquipmentItemPresenter augment2Slot;
    [SerializeField] private HeroEquipmentItemPresenter augment3Slot;
    
    public HeroEquipmentPanel Initialized(Hero h)
    {
        var equipment = h.Equipment;

        weaponSlot.Initialized(equipment.Weapon, () => BeginEquipmentSelection(h, equipment.Weapon));
        armorSlot.Initialized(equipment.Armor, () => BeginEquipmentSelection(h, equipment.Armor));
        InitAugmentSlots(h, equipment);
        return this;
    }

    private void InitAugmentSlots(Hero h, HeroEquipment equipment)
    {
        var augments = equipment.Augments;
        InitAugmentSlot1(h, augments);
        InitAugmentSlot2(h, augments);
        InitAugmentSlot3(h, augments);
    }

    private void InitAugmentSlot3(Hero h, Equipment[] augments)
    {
        var a3 = augments.Length > 2
            ? new Maybe<Equipment>(augments[2])
            : Maybe<Equipment>.Missing();
        augment3Slot.Initialized(a3, () => BeginEquipmentSelection(h, a3));
    }

    private void InitAugmentSlot2(Hero h, Equipment[] augments)
    {
        var a2 = augments.Length > 1
            ? new Maybe<Equipment>(augments[1])
            : Maybe<Equipment>.Missing();
        augment2Slot.Initialized(a2, () => BeginEquipmentSelection(h, a2));
    }

    private void InitAugmentSlot1(Hero h, Equipment[] augments)
    {
        var a1 = augments.Length > 0
            ? new Maybe<Equipment>(augments[0])
            : Maybe<Equipment>.Missing();
        augment1Slot.Initialized(a1, () => BeginEquipmentSelection(h, a1));
    }

    private void BeginEquipmentSelection(Hero h, Maybe<Equipment> equip)
    {
        equip.IfPresent(e => party.UnequipFrom(e, h));
        Message.Publish(new GetUserSelectedEquipment(party.Equipment.Available, 
            selection => selection.IfPresent(e => party.EquipTo(e, h))));
    }
}
