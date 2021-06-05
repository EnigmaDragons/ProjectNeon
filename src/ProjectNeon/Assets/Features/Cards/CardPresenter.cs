using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPresenter : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private BattleState battleState;
    [SerializeField] private CardRarityPresenter rarity;
    [SerializeField] private CardTargetPresenter target;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI type;
    [SerializeField] private Image art;
    [SerializeField] private Image tint;
    [SerializeField] private UnityEngine.UI.Extensions.Gradient tintGradient;
    [SerializeField] private GameObject canPlayHighlight;
    [SerializeField] private GameObject conditionMetHighlight;
    [SerializeField] private GameObject conditionNotMetHighlight;
    [SerializeField] private GameObject highlight;
    [SerializeField] private GameObject darken;
    [SerializeField] private CardControlsPresenter controls;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float dragScaleFactor = 1 / 0.7f;
    [SerializeField] private float highlightedScale = 1.7f;
    [SerializeField] private CardRulesPresenter rules;
    [SerializeField] private GameObject chainedCardParent;
    [SerializeField] private CardCostPresenter cardCostPresenter;
    [SerializeField] private Image[] glitchableComponents; 
    [SerializeField] private Material glitchMaterial;
    [SerializeField] private DragRotator dragRotator;
    [SerializeField] private GameObject[] onlyEnabledInHand;
    [SerializeField] private ArchetypeTints archetypeTints;
    [SerializeField] private Image background;
    [SerializeField] private Sprite standardCard;
    [SerializeField] private Sprite transientCard;

    private Card _card;
    private CardTypeData _cardType;
    private bool _debug = false;
    private int _preHighlightSiblingIndex;

    private Func<BattleState, Card, bool> _getCanPlay;
    private Func<bool> _getCanActivate;
    private Action _onClick;
    private Action _onDiscard;
    private Action _onMiddleMouse;
    private Action _onRightClick;
    private Action _onBeginDrag;
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

    public void Set(Card card) => Set("Library", card, () => { }, () => {}, () => { }, (_, __) => false, () => false);
    public void Set(Card card, Action onClick) => Set("Library", card, onClick, () => {}, () => { }, (_, __) => false, () => false);
    public void Set(CardTypeData card) => Set(card, () => { });
    
    public void Set(string zone, Card card, Action onClick, Action onBeginDrag, Action onDiscard, Func<BattleState, Card, bool> getCanPlay, Func<bool> getCanActivate)
    {
        InitFreshCard(onClick);

        _onDiscard = onDiscard;
        _onBeginDrag = onBeginDrag;
        _card = card;
        _cardType = card.Type;
        _getCanPlay = getCanPlay;
        _getCanActivate = getCanActivate;
        _zone = zone;
        _isHand = _zone.Contains("Hand");
        _onRightClick = _isHand ? ToggleAsBasic : (Action)(() => { });
        _requiresPlayerTargeting = _cardType.RequiresPlayerTargeting();
        RenderCardType();
    }
    
    public void Set(CardTypeData cardType, Action onClick)
    {
        InitFreshCard(onClick);

        _onBeginDrag = () => {};
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
        DisableCanPlayHighlight();
        DisableSelectedHighlight();
        _onClick = onClick;
        _onMiddleMouse = () => { };
        _onRightClick = () => { };
    }

    public void SetMiddleButtonAction(Action action) => _onMiddleMouse = action;

    public void ToggleAsBasic()
    {
        if (_card == null)
            throw new InvalidOperationException("Only Card Instances can be used as Basics. This Card Presenter does not have a Card instance.");
        
        DebugLog($"UI - Toggle as Basic");
        _card.TransitionTo(_card.Mode != CardMode.Basic ? CardMode.Basic : CardMode.Normal);
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
            DisableCanPlayHighlight();
            DisableSelectedHighlight();
            controls.SetActive(false);
        }
        darken.SetActive(isDisabled);
    }

    private void DisableCanPlayHighlight()
    {
        canPlayHighlight.SetActive(false);
        conditionMetHighlight.SetActive(false);
        conditionNotMetHighlight.SetActive(false);
    }
    
    private void SetCanPlayHighlight(bool highlightShouldBeActive, Maybe<bool> highlightCondition, Maybe<bool> unhighlightCondition)
    {
        DisableCanPlayHighlight();
        if (!highlightShouldBeActive)
            return;
        if (unhighlightCondition.IsPresentAnd(c => c))
            conditionNotMetHighlight.SetActive(true);
        else if (highlightCondition.IsPresentAnd(c => c))
            conditionMetHighlight.SetActive(true);
        else
            canPlayHighlight.SetActive(true);
    }

    private void DisableSelectedHighlight()
    {
        highlight.SetActive(false);
    }

    private void SetSelectedHighlight(bool highlightShouldBeActive)
    {
        DisableSelectedHighlight();
        if (!conditionMetHighlight && !conditionNotMetHighlight && highlightShouldBeActive)
            highlight.SetActive(true);
    }
    
    public void SetHandHighlight(bool active)
    {
        if (!highlight.activeSelf && !active && AreCloseEnough(transform.localScale.x, 1.0f))
            return;

        DebugLog($"Setting Selected Highlight {active}");
        SetSelectedHighlight(IsPlayable && active);
        controls.SetActive(active);
        SetSiblingIndex(active);
        
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
        if (active)
        {
            Message.Publish(new TweenMovementRequested(transform, new Vector3(0, sign * 180f, sign * 2f), tweenDuration, MovementDimension.Spatial, TweenMovementType.RubberBand, "Highlight"));
            Message.Publish(new TweenMovementRequested(transform, new Vector3(highlightedScale - 1f, highlightedScale - 1f, highlightedScale - 1f), tweenDuration, MovementDimension.Scale, TweenMovementType.RubberBand, "HighlightScale"));
        }
        else
        {
            Message.Publish(new SnapBackTweenRequested(transform, "Highlight"));  
            Message.Publish(new SnapBackTweenRequested(transform, "HighlightScale"));  
        }
        if (_card != null)
            if (active)
                Message.Publish(new HighlightCardOwner(_card.Owner));
            else
                Message.Publish(new UnhighlightCardOwner(_card.Owner));
    }

    private void SetSiblingIndex(bool active)
    {
        if (active && _preHighlightSiblingIndex == -1)
        {
            _preHighlightSiblingIndex = transform.GetSiblingIndex();
            transform.SetAsLastSibling();
        }
        else if (!active && _preHighlightSiblingIndex != -1)
        {
            transform.SetSiblingIndex(_preHighlightSiblingIndex);
            _preHighlightSiblingIndex = -1;
        }
    }

    public void ShowComprehensiveCardInfo()
    {
        rules.Show(_cardType);

        _cardType.ChainedCard.IfPresent(chain =>
        {
            if (_isHand)
                Message.Publish(new ShowReferencedCard(chainedCardParent, new Card(-1, _card.Owner, chain, _card.Tint)));
            else
                Message.Publish(new ShowReferencedCard(chainedCardParent, chain));
        });
        _cardType.SwappedCard.IfPresent(swap =>
        {
            if (_isHand)
                Message.Publish(new ShowReferencedCard(chainedCardParent, new Card(-1, _card.Owner, swap)));
            else
                Message.Publish(new ShowReferencedCard(chainedCardParent, swap));
        });
    }
    
    private void HideComprehensiveCardInfo()
    {
        rules.Hide();
        Message.Publish(new HideReferencedCard());
    }
    
    public void SetHighlightGraphicState(bool active) => highlight.SetActive(active);

    public void TeleportTo(Vector3 targetPosition)
    {
        Message.Publish(new StopMovementTweeningRequested(transform, MovementDimension.Spatial));
        transform.position = targetPosition;
        _position = targetPosition;
    }
    
    public void SetTargetPosition(Vector3 targetPosition)
    {
        DebugLog($"Set Target Position {targetPosition.ToString()}");
        _position = targetPosition;
        Message.Publish(new GoToTweenRequested(transform, targetPosition, Vector3.Distance(transform.position, targetPosition) / 1600f, MovementDimension.Spatial));
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
        SetCardTint();
        SetCanPlayHighlight(IsPlayable, 
            _card != null 
                ? _cardType.HighlightCondition.Map(condition => condition.ConditionMet(new CardConditionContext(_card, battleState))) 
                : Maybe<bool>.Missing(),
            _card != null 
                ? _cardType.UnhighlightCondition.Map(condition => condition.ConditionMet(new CardConditionContext(_card, battleState))) 
                : Maybe<bool>.Missing());
        if (_card == null || _card.Mode != CardMode.Glitched)
            glitchableComponents.ForEach(x => x.material = null);
        else 
            glitchableComponents.ForEach(x => x.material = glitchMaterial);
        dragRotator.Reset();
        dragRotator.enabled = _isHand;
        onlyEnabledInHand.ForEach(o => o.SetActive(_isHand));
        background.sprite = _card?.IsSinglePlay ?? _cardType.IsSinglePlay ? transientCard : standardCard;
    }

    private void SetCardTint()
    {
        tint.color = _card == null
            ? Color.white
            : _card.Tint;
        tintGradient.enabled = _card == null;
        if (_cardType.Archetypes.Count == 0)
        {
            var archetypeTint = archetypeTints.ForArchetypes(new HashSet<string>());
            tintGradient.Vertex1 = archetypeTint;
            tintGradient.Vertex2 = archetypeTint;
        }
        else if (_cardType.Archetypes.Count == 1)
        {
            var archetypeTint = archetypeTints.ForArchetypes(_cardType.Archetypes);
            tintGradient.Vertex1 = archetypeTint;
            tintGradient.Vertex2 = archetypeTint;
        }
        else
        {
            tintGradient.Vertex1 = archetypeTints.ForArchetypes(new HashSet<string>(new[] {_cardType.Archetypes.OrderBy(x => x).First()}));
            tintGradient.Vertex2 = archetypeTints.ForArchetypes(new HashSet<string>(new[] {_cardType.Archetypes.OrderBy(x => x).Last()}));
        }
    }

    private bool AreCloseEnough(float first, float second) => (first - second).IsFloatZero();

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
        if (_isHand && CheckIfCanPlay() && eventData.button == PointerEventData.InputButton.Left)
        {
            Cursor.visible = false;
            Message.Publish(new TweenMovementRequested(transform, new Vector3(0, 30f, 0), 0.03f, MovementDimension.Spatial, TweenMovementType.RubberBand, "Click"));
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
        DebugLog("UI - Pointer Up");
        if (_isHand && IsPlayable && eventData.button == PointerEventData.InputButton.Left)
        {
            Message.Publish(new SnapBackTweenRequested(transform, "Click"));
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
    {
        if (!_isDragging)
            eventData.pointerDrag = null;
        else
            WhenActivatableHand(() =>
            {
                if (!_requiresPlayerTargeting || !IsPlayable)
                    transform.localPosition = transform.localPosition + new Vector3(eventData.delta.x * dragScaleFactor,
                        eventData.delta.y * dragScaleFactor, 0);
            }, () => { });
    }

    public void OnBeginDrag(PointerEventData eventData)
        => WhenActivatableHand(() =>
        {
            _isDragging = true;
            controls.SetActive(false);
            canvasGroup.blocksRaycasts = false;

            // Targeting Card Selection Process can run the arrow
            if (_requiresPlayerTargeting && IsPlayable) 
                Message.Publish(new ShowMouseTargetArrow(transform));
            
            _onBeginDrag();
            Message.Publish(new BeginTargetSelectionRequested(_card));
        }, () => { });

    public void OnEndDrag(PointerEventData eventData) 
        => WhenActivatableHand(() =>
        {
            Message.Publish(new CancelTargetSelectionRequested());
            ReturnHandToNormal();
        }, UndoDragMovement);
    
    private void WhenActivatableHand(Action action, Action elseAction)
    {
        if (_isHand && _getCanActivate())
            action();
        else
            elseAction();
    }

    private void UndoDragMovement()
    {
        Cursor.visible = true;
    }
    
    private void ReturnHandToNormal()
    {
        _isDragging = false;
        canvasGroup.blocksRaycasts = true;
        Message.Publish(new HideMouseTargetArrow());
    }

    public void Cancel() => ReturnHandToNormal();
    
    public void Discard()
    {
        Debug.Log("Discard");
        ReturnHandToNormal();
        _onDiscard();
    }
    
    public void Activate()
    {
        Debug.Log("Activate");
        ReturnHandToNormal();
        _onClick();
    }

    #endregion
}
