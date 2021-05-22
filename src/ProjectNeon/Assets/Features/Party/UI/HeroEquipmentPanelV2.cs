using System;
using UnityEngine;

public class HeroEquipmentPanelV2 : MonoBehaviour
{
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private HeroEquipmentItemPresenter weaponSlot;
    [SerializeField] private HeroEquipmentItemPresenter armorSlot;
    [SerializeField] private HeroEquipmentItemPresenter augment1Slot;
    [SerializeField] private HeroEquipmentItemPresenter augment2Slot;
    [SerializeField] private HeroEquipmentItemPresenter augment3Slot;

    public HeroEquipmentPanelV2 Initialized()
    {
        var h = state.SelectedHeroesDeck.Hero;
        weaponSlot.Initialized(EquipmentSlot.Weapon, h.Equipment.Weapon, 
            () => h.Equipment.Weapon.IfPresent(e =>
            {
                party.UnequipFrom(e, h);
                Message.Publish(new EquipmentPicketCurrentGearChanged());
            }));
        armorSlot.Initialized(EquipmentSlot.Armor, h.Equipment.Armor, 
            () => h.Equipment.Armor.IfPresent(e =>
            {
                party.UnequipFrom(e, h);
                Message.Publish(new EquipmentPicketCurrentGearChanged());
            }));
        InitAugmentSlot(augment1Slot, 1);
        InitAugmentSlot(augment2Slot, 2);
        InitAugmentSlot(augment3Slot, 3);
        return this;
    }

    private void InitAugmentSlot(HeroEquipmentItemPresenter augmentSlot, int slot)
    {
        var h = state.SelectedHeroesDeck.Hero;
        var augments = h.Equipment.Augments;
        var a = augments.Length > slot - 1
            ? new Maybe<Equipment>(augments[slot - 1])
            : Maybe<Equipment>.Missing();
        augmentSlot.Initialized(EquipmentSlot.Augmentation, a,
            () => a.IfPresent(e =>
            {
                party.UnequipFrom(e, h);
                Message.Publish(new EquipmentPicketCurrentGearChanged());
            }));
    }
}