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
    [SerializeField] private CorpUiBase corpBranding;
    [SerializeField] private AllCorps allCorps;

    private Action _onClick = () => { };

    public void Set(Equipment e, Action onClick) => Initialized(e, onClick);
    
    public EquipmentPresenter Initialized(Equipment e, Action onClick)
    {
        _onClick = onClick;
        nameLabel.text = e.Name;
        slotLabel.text = $"{e.Slot}";
        var archetypeText = e.Archetypes.Any()
            ? string.Join(",", e.Archetypes.Select(c => c))
            : "Any";
        classesLabel.text = $"Archetypes: {archetypeText}";
        descriptionLabel.text = e.Description;
        rarity.Set(e.Rarity);
        slotIcon.sprite = slotIcons.All[e.Slot];
        corpBranding.Init(allCorps.GetCorpByNameOrNone(e.Corp));
        gameObject.SetActive(true);
        return this;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            _onClick();
    }
}
