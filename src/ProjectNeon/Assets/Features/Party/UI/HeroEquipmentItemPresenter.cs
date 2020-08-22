using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HeroEquipmentItemPresenter : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TextMeshProUGUI nameLabel;

    private Action _onClick = () => {};
    
    public HeroEquipmentItemPresenter Initialized(Maybe<Equipment> e, Action onClick)
    {
        _onClick = onClick;
        nameLabel.text = e.IsPresent ? e.Value.Name : "---";
        return this;
    }

    public void OnPointerDown(PointerEventData eventData) => _onClick();
}
