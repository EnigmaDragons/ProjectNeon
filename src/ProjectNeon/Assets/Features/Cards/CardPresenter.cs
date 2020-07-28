using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPresenter : MonoBehaviour, IPointerDownHandler
{
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
    [SerializeField] private GameObject darken;
    [SerializeField] private float highlightedScale = 1.7f;

    private Card _card;
    private CardType _cardType;
    
    private Action _onClick;
    private Vector3 _position;

    public bool Contains(Card c) => HasCard && c.Id == _card.Id;
    public bool Contains(CardType c) => HasCard && _cardType == c;
    public bool HasCard => _cardType != null;
    public bool IsPlayable => canPlayHighlight.activeSelf;

    public void ClearIfIs(Card c)
    {
        if (Contains(c))
            Clear();
    }
    
    public void ClearIfIs(CardType c)
    {
        if (Contains(c))
            Clear();
    }
    
    public void Clear()
    {
        gameObject.SetActive(false);
        _card = null;
        _cardType = null;
    }

    public void Set(Card card, Action onClick)
    {
        _card = card;
        Set(card.Type, onClick);
        description.text = card.InterpolatedDescription();
    }

    public void Set(CardType card, Action onClick)
    {
        gameObject.SetActive(true);
        canPlayHighlight.SetActive(false);
        _onClick = onClick;
        _cardType = card;
        
        nameLabel.text = _cardType.Name;
        description.text = _cardType.InterpolatedDescription(Maybe<Member>.Missing());
        type.text = _cardType.TypeDescription;
        art.sprite = _cardType.Art;
        
        var cost = card.Cost;
        costLabel.text = cost.Amount.ToString();
        costResourceTypeIcon.sprite = cost.ResourceType.Icon;
        costPanel.SetActive(!cost.ResourceType.Name.Equals("None") && cost.Amount > 0);

        card.LimitedToClass.IfPresent(c => tint.color = c.Tint);
    }

    public void ToggleAsBasic()
    {
        if (_card == null)
            throw new InvalidOperationException("Only Card Instances can be used as Basics. This Card Presenter does not have a Card instance.");
        SetAsBasic(!_card.UseAsBasic);
    }
    
    private void SetAsBasic(bool asBasic)
    {
        if (_card == null)
            throw new InvalidOperationException("Only Card Instances can be used as Basics. This Card Presenter does not have a Card instance.");
        var canPlay = canPlayHighlight.activeSelf;
        var useHighlight = highlight.activeSelf;
        
        _card.UseAsBasic = asBasic;
        Set(_card, _onClick);
        
        canPlayHighlight.SetActive(canPlay);
        highlight.SetActive(IsPlayable && useHighlight);
    }

    public void SetCanPlay(bool canPlay) => canPlayHighlight.SetActive(canPlay);
    public void SetDisabled(bool isDisabled)
    {
        if (isDisabled)
            SetCanPlay(false);
        darken.SetActive(isDisabled);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (battleState.SelectionStarted)
            return;
        if (eventData.button == PointerEventData.InputButton.Left)
            _onClick();
        if (_card != null && eventData.button == PointerEventData.InputButton.Right)
            ToggleAsBasic();
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
