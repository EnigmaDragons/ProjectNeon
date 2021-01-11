using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPresenter : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
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
    [SerializeField] private CardControlsPresenter controls;
    [SerializeField] private float highlightedScale = 1.7f;

    private Card _card;
    private CardTypeData _cardType;
    private bool _debug = false;
    private int _preHighlightSiblingIndex;

    private Func<BattleState, Card, bool> _getCanPlay;
    private Action _onClick;
    private Action _onMiddleMouse;
    private Vector3 _position;
    private string _zone;
    
    private bool IsHand => _zone.Contains("Hand");

    public string CardName => _cardType.Name;
    public bool Contains(Card c) => HasCard && c.Id == _card.Id;
    public bool Contains(CardTypeData c) => HasCard && _cardType.Name.Equals(c.Name);
    public bool HasCard => _cardType != null;
    public bool IsHighlighted => highlight.activeSelf;
    public bool IsPlayable { get; private set; }

    public void Clear()
    {
        gameObject.SetActive(false);
        _card = null;
        _cardType = null;
    }

    public void Set(Card card) => Set("Library", card, () => { }, (_, __) => false);
    
    public void Set(string zone, Card card, Action onClick, Func<BattleState, Card, bool> getCanPlay)
    {
        InitFreshCard(onClick);

        _card = card;
        _cardType = card.Type;
        _getCanPlay = getCanPlay;
        _zone = zone;
        RenderCardType();
    }
    
    public void Set(CardTypeData cardType, Action onClick)
    {
        InitFreshCard(onClick);

        _card = null;
        _cardType = cardType;
        _getCanPlay = (_, __) => false;
        _zone = "Library";
        RenderCardType();
    }

    private void InitFreshCard(Action onClick)
    {
        gameObject.SetActive(true);
        canPlayHighlight.SetActive(false);
        highlight.SetActive(false);
        _onClick = onClick;
        _onMiddleMouse = () => { };
    }

    public void SetMiddleButtonAction(Action action) => _onMiddleMouse = action;

    private string CostLabel(ResourceCost cost)
    {
        var owner = _card != null ? new Maybe<Member>(_card.Owner) : Maybe<Member>.Missing();
        var numericAmount = cost.BaseAmount.ToString();
        // Non-X Cost Cards
        if (!cost.PlusXCost)
            return numericAmount;

        // X Cost Cards
        if (owner.IsMissing)
            return $"{numericAmount}+X".Replace("0+", "");
        else
            return _card.LockedXValue.Select(
                r => r.Amount.ToString(),
                () => _card.Cost.XAmountSpent(owner.Value).Amount.ToString());
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
        
        DebugLog($"UI - Toggle as Basic {CardName}");
        
        _card.UseAsBasic = asBasic;
        _cardType = _card.Type;
        RenderCardType();
        if (IsPlayable)
            SetHandHighlight(true);
    }
    
    public void SetDisabled(bool isDisabled)
    {
        if (isDisabled)
        {
            DebugLog($"{CardName} is disabled.");
            canPlayHighlight.SetActive(false);
            controls.SetActive(false);
        }
        darken.SetActive(isDisabled);
    }
    
    public void SetHandHighlight(bool active)
    {
        if (!highlight.activeSelf && !active && AreCloseEnough(transform.localScale.x, 1.0f))
            return;
        
        DebugLog($"Setting Highlight {active}");
        controls.SetActive(active);
        if (active)
        {
            _preHighlightSiblingIndex = transform.GetSiblingIndex();
            transform.SetAsLastSibling();
        }
        else
        {
            transform.SetSiblingIndex(_preHighlightSiblingIndex);
        }

        highlight.SetActive(IsPlayable && active);
        var sign = active ? 1 : -1;
        var scale = active ? new Vector3(highlightedScale, highlightedScale, highlightedScale) : new Vector3(1f, 1f, 1f);
        var position = active ? _position + new Vector3(0, sign * 180f, sign * 2f) : _position;
        if (AreCloseEnough(scale.x, transform.localScale.x) && AreCloseEnough(position.y, transform.position.y))
            return;

        var tweenDuration = 0.18f;
        transform.DOScale(scale, tweenDuration);
        transform.DOMove(position, tweenDuration);
        if (_card != null)
            if (active)
                Message.Publish(new HighlightCardOwner(_card.Owner));
            else
                Message.Publish(new UnhighlightCardOwner(_card.Owner));
    }

    public void SetHighlightGraphicState(bool active) => highlight.SetActive(active);

    public void TeleportTo(Vector3 targetPosition)
    {
        transform.position = targetPosition;
        _position = targetPosition;
    }
    
    public void SetTargetPosition(Vector3 targetPosition)
    {
        _position = targetPosition;
        transform.DOMove(targetPosition, 1);
    }

    private void RenderCardType()
    {
        IsPlayable = CheckIfCanPlay();
        nameLabel.text = _cardType.Name;
        description.text = _card != null 
            ? _cardType.InterpolatedDescription(_card.Owner, _card.LockedXValue) 
            : _cardType.InterpolatedDescription(Maybe<Member>.Missing(), Maybe<ResourceQuantity>.Missing());
        type.text = _cardType.TypeDescription;
        art.sprite = _cardType.Art;
        rarity.Set(_cardType.Rarity);
        target.Set(_cardType);

        var cost = _cardType.Cost;
        var hasCost = !cost.ResourceType.Name.Equals("None") && cost.BaseAmount > 0 || cost.PlusXCost;
        costPanel.SetActive(hasCost);
        if (hasCost)
        {
            costLabel.text = CostLabel(cost);
            costResourceTypeIcon.sprite = cost.ResourceType.Icon;
        }

        tint.color = _cardType.LimitedToClass.Select(c => c.Tint, () => Color.white);
        canPlayHighlight.SetActive(IsPlayable);
        highlight.SetActive(IsPlayable);
    }
    
    private bool AreCloseEnough(float first, float second) => WithinEpsilon(first - second);
    private bool WithinEpsilon(float f) => Math.Abs(f) < 0.05;

    private bool CheckIfCanPlay()
    {
        var result = _card != null && _getCanPlay(battleState, _card);
        DebugLog($"Can Play {CardName}: {result}");
        return result;
    }

    private void DebugLog(string msg)
    {
        if (_debug)
            Log.Info(msg);
    }
    
    #region Mouse Controls
    public void OnPointerDown(PointerEventData eventData)
    { 
        DebugLog($"UI - Pointer Down - {CardName}");
        if (battleState.IsSelectingTargets)
            return;
        if (eventData.button == PointerEventData.InputButton.Left)
            _onClick();
        if (eventData.button == PointerEventData.InputButton.Middle) 
            _onMiddleMouse();
        if (IsHand && _card != null && eventData.button == PointerEventData.InputButton.Right)
            ToggleAsBasic();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsHand)
            Message.Publish(new CardHoverEnter(this));
    }

    public void OnPointerExit(PointerEventData eventData) => SetHandHighlight(false);

    #endregion
}
