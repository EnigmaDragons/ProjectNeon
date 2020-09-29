using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPresenter : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private BattleState battleState;
    [SerializeField] private CardRarityPresenter rarity;
    [SerializeField] private CardTargetPresenter target;
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
    [SerializeField] private GameObject controls;
    [SerializeField] private float highlightedScale = 1.7f;

    private Card _card;
    private CardTypeData _cardType;

    private bool _canHighlight;
    private Action _onClick;
    private Vector3 _position;

    public bool Contains(Card c) => HasCard && c.Id == _card.Id;
    public bool Contains(CardType c) => HasCard && _cardType == c;
    public bool HasCard => _cardType != null;
    public bool IsPlayable => canPlayHighlight.activeSelf;
    public bool IsHighlighted => highlight.activeSelf;

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
    
    public void Set(Card card, Action onClick, bool canHighlight = false)
    {
        _card = card;
        Set(card.Type, onClick, canHighlight);
        description.text = card.InterpolatedDescription();
    }

    public void Set(CardTypeData card, Action onClick, bool canHighlight = false)
    {
        gameObject.SetActive(true);
        canPlayHighlight.SetActive(false);
        highlight.SetActive(false);
        _canHighlight = canHighlight;
        _onClick = onClick;
        _cardType = card;
        
        nameLabel.text = _cardType.Name;
        description.text = _cardType.InterpolatedDescription(Maybe<Member>.Missing());
        if (string.IsNullOrWhiteSpace(_cardType.TypeDescription))
            Log.Error($"{_cardType} is missing it's Type Description");
        type.text = _cardType.TypeDescription;
        art.sprite = _cardType.Art;
        rarity.Set(card.Rarity);
        target.Set(card);
        
        var cost = card.Cost;
        var hasCost = !cost.ResourceType.Name.Equals("None") && cost.BaseAmount > 0 || cost.PlusXCost;
        costPanel.SetActive(hasCost);
        if (hasCost)
        {
            costLabel.text = CostLabel(cost);
            costResourceTypeIcon.sprite = cost.ResourceType.Icon;
        }

        card.LimitedToClass.IfPresent(c => tint.color = c.Tint);
    }

    private string CostLabel(ResourceCost cost)
    {
        var numericAmount = cost.BaseAmount.ToString();
        if (!cost.PlusXCost)
            return numericAmount;
        else if (cost.BaseAmount == 0)
            return "X";
        else
            return $"{numericAmount}+X";
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
        {
            SetCanPlay(false);
            SetCardHandControlsVisible(false);
        }
        darken.SetActive(isDisabled);
    }

    public void SetCardHandControlsVisible(bool isActive) => controls.SetActive(isActive);

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
        if (!highlight.activeSelf && !active && AreCloseEnough(transform.localScale.x, 1.0f))
            return;

        if (active)
            transform.SetAsLastSibling();
        highlight.SetActive(IsPlayable && active);
        var sign = active ? 1 : -1;
        var scale = active ? new Vector3(highlightedScale, highlightedScale, highlightedScale) : new Vector3(1f, 1f, 1f);
        var position = active ? _position + new Vector3(0, sign * 100f, sign * 2f) : _position;
        if (AreCloseEnough(scale.x, transform.localScale.x) && AreCloseEnough(position.y, transform.position.y))
            return;

        // TODO: Track down why cards move when switching between Basic and Normal
        // if (_cardType != null)
        //    Log.Info($"Moving Card {_cardType.Name} to {active} Highlighted position {position}. Target Position is {_position}");
        transform.DOScale(scale, 0.4f);
        transform.DOMove(position, 0.4f);
    }

    public void SetHighlightGraphicState(bool active) => highlight.SetActive(active);

    public void MoveTo(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }
    
    public void SetTargetPosition(Vector3 targetPosition)
    {
        _position = targetPosition;
        transform.DOMove(targetPosition, 1);
    }

    private bool AreCloseEnough(float first, float second) => WithinEpsilon(first - second);
    private bool WithinEpsilon(float f) => Math.Abs(f) < 0.05;
}
