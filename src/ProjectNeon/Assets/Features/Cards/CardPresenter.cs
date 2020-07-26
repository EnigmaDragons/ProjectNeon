using System;
using DG.Tweening;
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
    [SerializeField] private GameObject costPanel;
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private Image costResourceTypeIcon;
    [SerializeField] private Image art;
    [SerializeField] private Image tint;
    [SerializeField] private GameObject canPlayHighlight;
    [SerializeField] private GameObject highlight;
    [SerializeField] private float highlightedScale = 1.7f;

    private Card _card;
    private Action _onClick;
    private Vector3 _position;

    public bool Contains(Card c) => HasCard && _card == c;
    public bool HasCard => _card != null;
    public bool IsPlayable => canPlayHighlight.activeSelf;

    public void ClearIfIs(Card c)
    {
        if (Contains(c))
            Clear();
    }
    
    public void Clear()
    {
        gameObject.SetActive(false);
        _card = null;
    }

    public void Set(Card card, Action onClick)
    {
        gameObject.SetActive(true);
        canPlayHighlight.SetActive(false);
        _onClick = onClick;
        _card = card;
        
        nameLabel.text = _card.Name;
        description.text = _card.Description;
        type.text = _card.TypeDescription;
        art.sprite = _card.Art;
        
        var cost = card.Cost;
        costLabel.text = cost.Amount.ToString();
        costResourceTypeIcon.sprite = cost.ResourceType.Icon;
        costPanel.SetActive(!cost.ResourceType.Name.Equals("None") && cost.Amount > 0);
        
        tint.color = classTints.TintFor(card.LimitedToClass.OrDefault(() => ""));
    }

    public void SetCanPlay(bool canPlay) => canPlayHighlight.SetActive(canPlay);
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (battleState.SelectionStarted)
            return;
        _onClick();
    }

    public void SetHighlight(bool active)
    {
        if (!highlight.activeSelf && !active&& Math.Abs(transform.localScale.x - 1.0f) < 0.05)
            return;
        
        highlight.SetActive(IsPlayable && active);
        var sign = active ? 1 : -1;
        var scale = active ? new Vector3(highlightedScale, highlightedScale, highlightedScale) : new Vector3(1f, 1f, 1f);
        var position = active ? _position + new Vector3(0, sign * 100f, sign * 2f) : _position;
        transform.DOScale(scale, 0.4f);
        transform.DOMove(position, 0.4f);
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        _position = targetPosition;
    }
}
