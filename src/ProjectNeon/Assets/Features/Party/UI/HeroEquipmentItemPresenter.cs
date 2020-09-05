using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroEquipmentItemPresenter : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private Image slotIcon;
    [SerializeField] private EquipmentSlotIcons icons;

    private Action _onClick = () => {};
    
    public HeroEquipmentItemPresenter Initialized(EquipmentSlot slot, Maybe<Equipment> e, Action onClick)
    {
        _onClick = onClick;
        nameLabel.text = e.IsPresent ? e.Value.Name : "---";
        slotIcon.sprite = icons.All[slot];
        return this;
    }

    public void OnPointerDown(PointerEventData eventData) => _onClick();
}
