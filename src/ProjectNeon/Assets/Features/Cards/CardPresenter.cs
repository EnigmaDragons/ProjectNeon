using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPresenter : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private const float _clickMoveDistance = 30f;
    private const float _clickTweenSpeed = 0.03f;
    
    [SerializeField] private BattleState battleState;
    [SerializeField] private CardRarityPresenter rarity;
    [SerializeField] private CardTargetPresenter target;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI type;
    [SerializeField] private Image art;
    [SerializeField] private Image tint;
    [SerializeField] private GameObject canPlayHighlight;
    [SerializeField] private GameObject highlight;
    [SerializeField] private GameObject darken;
    [SerializeField] private CardControlsPresenter controls;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float dragScaleFactor = 1 / 0.7f;
    [SerializeField] private float highlightedScale = 1.7f;
    [SerializeField] private CardRulesPresenter rules;
    [SerializeField] private GameObject chainedCardParent;
    [SerializeField] private CardCostPresenter cardCostPresenter;

    private Card _card;
    private CardTypeData _cardType;
    private bool _debug = false;
    private int _preHighlightSiblingIndex;

    private Func<BattleState, Card, bool> _getCanPlay;
    private Action _onClick;
    private Action _onMiddleMouse;
    private Action _onRightClick;
    private Vector3 _position;
    
    // Hand 
    private string _zone;
    private bool _isHand;
    
    private bool _requiresPlayerTargeting;

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
    public void Set(CardTypeData card) => Set(card, () => { });
    
    public void Set(string zone, Card card, Action onClick, Func<BattleState, Card, bool> getCanPlay)
    {
        InitFreshCard(onClick);

        _card = card;
        _cardType = card.Type;
        _getCanPlay = getCanPlay;
        _onRightClick = ToggleAsBasic;
        _zone = zone;
        _isHand = _zone.Contains("Hand");
        _requiresPlayerTargeting = _cardType.RequiresPlayerTargeting();
        RenderCardType();
    }
    
    public void Set(CardTypeData cardType, Action onClick)
    {
        InitFreshCard(onClick);

        _card = null;
        _cardType = cardType;
        _getCanPlay = (_, __) => false;
        _onRightClick = _cardType.ShowDetailedCardView;
        _zone = "Library";
        _isHand = false;
        _requiresPlayerTargeting = false;
        RenderCardType();
    }

    private void InitFreshCard(Action onClick)
    {
        gameObject.SetActive(true);
        canPlayHighlight.SetActive(false);
        highlight.SetActive(false);
        _onClick = onClick;
        _onMiddleMouse = () => { };
        _onRightClick = () => { };
    }

    public void SetMiddleButtonAction(Action action) => _onMiddleMouse = action;

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
        
        DebugLog($"UI - Toggle as Basic");
        
        _card.UseAsBasic = asBasic;
        _cardType = _card.Type;
        _requiresPlayerTargeting = _cardType.RequiresPlayerTargeting();
        RenderCardType();
        if (IsPlayable)
            SetHandHighlight(true);
    }
    
    public void SetDisabled(bool isDisabled)
    {
        if (isDisabled)
        {
            DebugLog($"is disabled.");
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
        if (active)
            ShowComprehensiveCardInfo();
        else
            HideComprehensiveCardInfo();

        var sign = active ? 1 : -1;
        var scale = active ? new Vector3(highlightedScale, highlightedScale, highlightedScale) : new Vector3(1f, 1f, 1f);
        var position = active ? _position + new Vector3(0, sign * 180f, sign * 2f) : _position;
        if (AreCloseEnough(scale.x, transform.localScale.x) && AreCloseEnough(position.y, transform.position.y))
            return;

        var tweenDuration = 0.08f;
        DebugLog($"Tweening Highlight {active}");
        transform.DOScale(scale, tweenDuration);
        transform.DOMove(position, tweenDuration);
        if (_card != null)
            if (active)
                Message.Publish(new HighlightCardOwner(_card.Owner));
            else
                Message.Publish(new UnhighlightCardOwner(_card.Owner));
    }

    public void ShowComprehensiveCardInfo()
    {
        rules.Show(_cardType);

        _cardType.ChainedCard.IfPresent(chain =>
        {
            if (_isHand)
                Message.Publish(new ShowChainedCard(chainedCardParent, new Card(-1, _card.Owner, chain)));
            else
                Message.Publish(new ShowChainedCard(chainedCardParent, chain));
        });
    }
    
    private void HideComprehensiveCardInfo()
    {
        rules.Hide();
        Message.Publish(new HideChainedCard());
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
            ? _cardType.InterpolatedDescription(_card.Owner, _card.LockedXValue.OrDefault(() => _card.Owner.CalculateResources(_card.Type).XAmountQuantity)) 
            : _cardType.InterpolatedDescription(Maybe<Member>.Missing(), ResourceQuantity.None);
        type.text = _cardType.TypeDescription;
        art.sprite = _cardType.Art;
        rarity.Set(_cardType.Rarity);
        target.Set(_cardType);
        cardCostPresenter.Render(_card, _cardType);
        tint.color = _cardType.LimitedToClass.Select(c => c.Tint, () => Color.white);
        canPlayHighlight.SetActive(IsPlayable);
        highlight.SetActive(IsPlayable);
    }
    
    private bool AreCloseEnough(float first, float second) => WithinEpsilon(first - second);
    private bool WithinEpsilon(float f) => Math.Abs(f) < 0.05;

    private bool CheckIfCanPlay()
    {
        var result = _card != null && _getCanPlay(battleState, _card);
        DebugLog($"Can Play: {result}");
        return result;
    }

    private void DebugLog(string msg)
    {
        if (_debug)
            Log.Info($"Card {CardName}: {msg}");
    }
    
    #region Mouse Controls
    public void MiddleClick() => _onMiddleMouse();
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.dragging)
            return;
        DebugLog($"UI - Pointer Down - {CardName}");
        if (_isHand && IsPlayable && eventData.button == PointerEventData.InputButton.Left)
        {
            Cursor.visible = false;
            transform.DOMove(transform.position + new Vector3(0, _clickMoveDistance, 0), _clickTweenSpeed);
        }
        if (!_isHand && eventData.button == PointerEventData.InputButton.Left)
            _onClick();
        if (eventData.button == PointerEventData.InputButton.Middle)
            _onMiddleMouse();
        if (eventData.button == PointerEventData.InputButton.Right)
            _onRightClick();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Cursor.visible = true;
        if (_isHand && IsPlayable && eventData.button == PointerEventData.InputButton.Left)
        {
            transform.DOMove(transform.position + new Vector3(0, -_clickMoveDistance, 0), _clickTweenSpeed);
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!eventData.dragging && _isHand && battleState.Phase == BattleV2Phase.PlayCards)
            Message.Publish(new CardHoverEnter(this));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isDragging && _isHand)
            SetHandHighlight(false);
    }

    private bool _isDragging = false;
    
    public void OnDrag(PointerEventData eventData)
        => WhenPlayableHand(() =>
        {
            if (!_requiresPlayerTargeting)
                transform.localPosition = transform.localPosition + new Vector3(eventData.delta.x * dragScaleFactor, eventData.delta.y * dragScaleFactor, 0);
        });
    
    public void OnBeginDrag(PointerEventData eventData)
        => WhenPlayableHand(() =>
        {
            _isDragging = true;
            canvasGroup.blocksRaycasts = false;
        
            // Targeting Card Selection Process can run the arrow
            if (_requiresPlayerTargeting)
                Message.Publish(new ShowMouseTargetArrow(new Vector3(0, 2f, 0)));
            Message.Publish(new BeginTargetSelectionRequested(_card));
        });

    public void OnEndDrag(PointerEventData eventData) => WhenPlayableHand(ReturnHandToNormal);

    private void WhenPlayableHand(Action action)
    {
        if (_isHand && IsPlayable)
            action();
    }

    private void ReturnHandToNormal()
    {
        _isDragging = false;
        canvasGroup.blocksRaycasts = true;
        Message.Publish(new HideMouseTargetArrow());
    }

    public void Activate()
    {
        ReturnHandToNormal();
        _onClick();
    }

    #endregion
}
