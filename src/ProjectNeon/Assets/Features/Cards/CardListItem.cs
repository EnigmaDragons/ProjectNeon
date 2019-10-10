using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardListItem : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private Image art;
    [SerializeField] private TextMeshProUGUI count;
    [SerializeField] private GameObject highlight;
    [SerializeField] private CardSelector selector;


    private Card _card;
    public Card Card => this._card;

    private Action _onClick;

    public void Set(Card card, Action onClick)
    {
        _onClick = onClick;
        _card = card;
        name.text = _card.name.WithSpaceBetweenWords();
        art.sprite = _card.Art;
        count.text = "2x";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerClick(eventData);
    }

    public void SetHighlight(bool active)
    {
        highlight.SetActive(active);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        this.selector.Init(Card);
        this.selector.ListToDeck();
    }
}
