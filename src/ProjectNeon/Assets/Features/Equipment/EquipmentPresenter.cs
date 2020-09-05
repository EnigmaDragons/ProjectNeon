using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentPresenter : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI slotLabel;
    [SerializeField] private TextMeshProUGUI descriptionLabel;
    [SerializeField] private TextMeshProUGUI classesLabel;
    [SerializeField] private CardRarityPresenter rarity;
    [SerializeField] private Image slotIcon;
    [SerializeField] private EquipmentSlotIcons slotIcons;

    private Action _onClick = () => { };
    
    public EquipmentPresenter Initialized(Equipment e, Action onClick)
    {
        _onClick = onClick;
        nameLabel.text = e.Name;
        slotLabel.text = $"{e.Slot}";
        var classesText = e.Classes.Any(c => c.Equals(CharacterClass.All))
            ? CharacterClass.All
            : string.Join(",", e.Classes.Select(c => c));
        classesLabel.text = $"Classes: {classesText}";
        descriptionLabel.text = e.Description;
        rarity.Set(e.Rarity);
        slotIcon.sprite = slotIcons.All[e.Slot];
        gameObject.SetActive(true);
        return this;
    }

    public void OnPointerDown(PointerEventData eventData) => _onClick();
}
