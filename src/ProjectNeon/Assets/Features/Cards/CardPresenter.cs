using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPresenter : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private ClassTints classTints;
    [SerializeField] private BattleState battleState;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI type;
    [SerializeField] private Image art;
    [SerializeField] private GameObject highlight;
    [SerializeField] private Image tint;

    private Card _card;
    private Action _onClick;
    public void Set(Card card, Action onClick)
    {
        _onClick = onClick;
        _card = card;
        nameLabel.text = _card.Name;
        description.text = _card.Description;
        type.text = _card.TypeDescription;
        art.sprite = _card.Art;
        tint.color = classTints.TintFor(card.LimitedToClass.OrDefault(() => ""));
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (battleState.SelectionStarted)
            return;
        _onClick();
    }

    public void SetHighlight(bool active)
    {
        highlight.SetActive(active);
    }
}
