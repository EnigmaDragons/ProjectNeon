using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardListItem : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private Image art;
    [SerializeField] private TextMeshProUGUI count;

    [SerializeField] private GameObject highlight;

    private CardType _card;
    private Action _onClick;

    public void Set(CardType card, Action onClick)
    {
        _onClick = onClick;
        _card = card;
        name.text = _card.name.WithSpaceBetweenWords();
        art.sprite = _card.Art;
        count.text = "2x";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //_onClick();
    }

    public void SetHighlight(bool active)
    {
        highlight.SetActive(active);
    }
}
