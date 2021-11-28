using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentPresenter : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject highlight;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI slotLabel;
    [SerializeField] private TextMeshProUGUI descriptionLabel;
    [SerializeField] private TextMeshProUGUI classesLabel;
    [SerializeField] private RarityPresenter rarity;
    [SerializeField] private Image slotIcon;
    [SerializeField] private EquipmentSlotIcons slotIcons;
    [SerializeField] private CorpUiBase corpBranding;
    [SerializeField] private AllCorps allCorps;

    private bool _useHoverHighlight = false;
    private Action _onClick = () => { };

    public void Set(Equipment e, Action onClick) => Initialized(e, onClick);
    
    public EquipmentPresenter Initialized(Equipment e, Action onClick, bool useHoverHighlight = false)
    {
        _onClick = onClick;
        _useHoverHighlight = useHoverHighlight;
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
        highlight.SetActive(false);
        gameObject.SetActive(true);
        return this;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            _onClick();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_useHoverHighlight)
            highlight.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_useHoverHighlight)
            highlight.SetActive(false);
    }
}
