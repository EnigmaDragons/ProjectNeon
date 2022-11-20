using System;
using I2.Loc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroEquipmentItemPresenter : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Localize nameLabel;
    [SerializeField] private Image slotIcon;
    [SerializeField] private EquipmentSlotIcons icons;

    private Action _onClick = () => {};
    
    public HeroEquipmentItemPresenter Initialized(EquipmentSlot slot, Maybe<Equipment> e, Action onClick)
    {
        _onClick = onClick;
        nameLabel.SetTerm(e.IsPresent ? e.Value.LocalizationNameTerm() : "---");
        slotIcon.sprite = icons.All[slot];
        return this;
    }

    public void OnPointerDown(PointerEventData eventData) => _onClick();
}
