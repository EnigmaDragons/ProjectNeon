
using System;
using System.Linq;
using UnityEngine;

public class HeroEquipmentPanel : MonoBehaviour
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private HeroEquipmentItemPresenter weaponSlot;
    [SerializeField] private HeroEquipmentItemPresenter armorSlot;
    [SerializeField] private HeroEquipmentItemPresenter augment1Slot;
    [SerializeField] private HeroEquipmentItemPresenter augment2Slot;
    [SerializeField] private HeroEquipmentItemPresenter augment3Slot;

    private Action _doNothing = () => { };
    
    public HeroEquipmentPanel Initialized(Hero h, bool isReadOnly = false)
    {
        var equipment = h.Equipment;

        weaponSlot.Initialized(EquipmentSlot.Weapon, equipment.Weapon, 
            isReadOnly ? _doNothing : () => BeginEquipmentSelection(h, EquipmentSlot.Weapon, equipment.Weapon));
        armorSlot.Initialized(EquipmentSlot.Armor, equipment.Armor, 
            isReadOnly ? _doNothing : () => BeginEquipmentSelection(h, EquipmentSlot.Armor, equipment.Armor));
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

    private void InitAugmentSlot3(Hero h, StaticEquipment[] augments)
    {
        var a3 = augments.Length > 2
            ? new Maybe<StaticEquipment>(augments[2])
            : Maybe<StaticEquipment>.Missing();
        augment3Slot.Initialized(EquipmentSlot.Augmentation, a3, () => BeginEquipmentSelection(h, EquipmentSlot.Augmentation, a3));
    }

    private void InitAugmentSlot2(Hero h, StaticEquipment[] augments)
    {
        var a2 = augments.Length > 1
            ? new Maybe<StaticEquipment>(augments[1])
            : Maybe<StaticEquipment>.Missing();
        augment2Slot.Initialized(EquipmentSlot.Augmentation, a2, () => BeginEquipmentSelection(h, EquipmentSlot.Augmentation, a2));
    }

    private void InitAugmentSlot1(Hero h, StaticEquipment[] augments)
    {
        var a1 = augments.Length > 0
            ? new Maybe<StaticEquipment>(augments[0])
            : Maybe<StaticEquipment>.Missing();
        augment1Slot.Initialized(EquipmentSlot.Augmentation, a1, () => BeginEquipmentSelection(h, EquipmentSlot.Augmentation, a1));
    }

    private void BeginEquipmentSelection(Hero h, EquipmentSlot slot, Maybe<StaticEquipment> equip)
    {
        equip.IfPresent(e => party.UnequipFrom(e, h));
        var equipmentOptions = party.Equipment.AvailableFor(h.Character)
            .Where(e => e.Slot == slot);
        Message.Publish(new GetUserSelectedEquipment(equipmentOptions, 
            selection => selection.IfPresent(e => party.EquipTo(e, h))));
    }
}
