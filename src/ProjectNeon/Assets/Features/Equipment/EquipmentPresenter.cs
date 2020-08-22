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

    private Action _onClick = () => { };
    
    public EquipmentPresenter Initialized(Equipment e, Action onClick)
    {
        _onClick = onClick;
        nameLabel.text = e.Name;
        slotLabel.text = $"{e.Slot}";
        var classesText = e.Classes.Any(c => c.Name.Equals("None"))
            ? "All"
            : string.Join(",", e.Classes.Select(c => c.Name));
        classesLabel.text = $"Classes: {classesText}";
        descriptionLabel.text = e.Description;
        gameObject.SetActive(true);
        return this;
    }

    public void OnPointerDown(PointerEventData eventData) => _onClick();
}
