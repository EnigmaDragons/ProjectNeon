using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPresenter : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private BattleState battleState;
    [SerializeField] private RarityPresenter rarity;
    [SerializeField] private CardTargetPresenter target;
    
    [SerializeField] private Localize nameLabel;
    [SerializeField] private Localize description;
    [SerializeField] private Localize typeLabel;
    [SerializeField] private Image art;
    [SerializeField] private Image tint;
    [SerializeField] private CardBustPresenter bust;
    [SerializeField] private UnityEngine.UI.Extensions.Gradient tintGradient;
    [SerializeField] private GameObject canPlayHighlight;
    [SerializeField] private GameObject conditionMetHighlight;
    [SerializeField] private GameObject conditionNotMetHighlight;
    [SerializeField] private GameObject highlight;
    [SerializeField] private Image fullScreenDarken;
    [SerializeField] private Color fullScreenDarkenFinalColor = new Color(0, 0, 0, 0.25f);
    [SerializeField] private GameObject darken;
    [SerializeField] private CardControlsPresenter controls;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float dragScaleFactor = 1 / 0.7f;
    [SerializeField] private float highlightedScale = 1.7f;
    [SerializeField] private CardRulesPresenter rules;
    [SerializeField] private CardTargetRulePresenter targetRule;
    [SerializeField] private CardScaledStatsPresenter scalingRule;
    [SerializeField] private CardEnemyTypePresenter enemyTypePresenter;
    [SerializeField] private GameObject referencedCardParent1;
    [SerializeField] private GameObject referencedCardParent2detailed;
    [SerializeField] private GameObject referencedCardParent2inHand;
    [SerializeField] private CardCostPresenter cardCostPresenter;
    [SerializeField] private Image[] glitchableComponents; 
    [SerializeField] private Material glitchMaterial;
    [SerializeField] private DragRotator dragRotator;
    [SerializeField] private GameObject[] onlyEnabledInHand;
    [SerializeField] private ArchetypeTints archetypeTints;
    [SerializeField] private Image background;
    [SerializeField] private Sprite standardCard;
    [SerializeField] private Sprite transientCard;
    [SerializeField] private AllCards allCards;
    [SerializeField] private CardPlayZone handZone;
    [SerializeField] private BattleState state;
    
    private bool _debug = false;

    private const string HandString = "Hand";
    private const string LibraryString = "Library";
    
    private Card _card;
    private CardTypeData _cardType;

    private Func<BattleState, Card, bool> _getCanPlay;
    private Func<bool> _getCanActivate;
    private Action _onClick;
    private Action _onDiscard;
    private Action _onMiddleMouse;
    private Action _onRightClick;
    private Action _onBeginDrag;
    private Vector3 _position;
    
    private bool _useCustomHoverActions;
    private Action _hoverEnterAction;
    private Action _hoverExitAction;
    
    // Drag Area
    private readonly Vector2 _dragPaddingFactor = new Vector2(0.05f, 0.05f);
    private readonly Vector2 _dragOffset = new Vector2(0, -40);
    private Vector2 _minDragPoint = new Vector2(0, 0);
    private Vector2 _maxDragPoint = new Vector2(1920, 1280);
    
    // Hand 
    private string _zone;
    private bool _isHand;
    private int _siblingIndex = -1;
    
    private bool _requiresPlayerTargeting;

    public bool Contains(Card c) => HasCard && c.CardId == _card.CardId;
    public bool Contains(CardTypeData c) => HasCard && _cardType.Name.Equals(c.Name);
    public bool HasCard => _cardType != null;
    public bool IsFocused { get; private set; }
    public bool IsPlayable { get; private set; }
    public bool IsDragging { get; private set; } = false;
    
    public string CardName => _cardType?.Name ?? string.Empty;
    
    private void OnEnable()
    {
        Message.Subscribe<CardHighlighted>(OnCardHighlighted, this);
        Message.Subscribe<MemberUnconscious>(_ => UpdateCardHighlight(), this);
        Message.Subscribe<LanguageChanged>(_ => RenderCardType(), this);
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }

    private void OnCardHighlighted(CardHighlighted e)
    {
        if (IsFocused && e.CardPresenter != this)
            SetHandHighlight(false);
    }

    public void Clear()
    {
        gameObject.SetActive(false);
        IsFocused = false;
        _card = null;
        _cardType = null;
        UpdateDragArea();
        _siblingIndex = -1;
    }

    private void UpdateDragArea()
    {
        var screenWidth = Screen.width;
        var screenHeight = Screen.height;
        var xPadding = screenWidth * _dragPaddingFactor.x;
        var yPadding = screenHeight * _dragPaddingFactor.y;
        _minDragPoint = new Vector2(0 + _dragOffset.x + xPadding, 0 + _dragOffset.y + yPadding);
        _maxDragPoint = new Vector2(screenWidth + _dragOffset.x - xPadding, screenHeight + _dragOffset.y - yPadding);
    }

    public void Set(Card card) => Set(LibraryString, card, () => { }, () => {}, () => { }, (_, __) => false, () => false);
    public void Set(Card card, Action onClick) => Set(LibraryString, card, onClick, () => {}, () => { }, (_, __) => false, () => false);
    public void Set(CardTypeData card) => Set(card, () => { });
    
    public void Set(string zone, Card card, Action onClick, Action onBeginDrag, Action onDiscard, Func<BattleState, Card, bool> getCanPlay, Func<bool> getCanActivate)
    {
        if (_debug)
            DebugLog($"Card Set - {card.Name}");
        InitFreshCard(onClick);

        _onDiscard = onDiscard;
        _onBeginDrag = onBeginDrag;
        _card = card;
        _cardType = card.Type;
        _getCanPlay = getCanPlay;
        _getCanActivate = getCanActivate;
        _zone = zone;
        _isHand = _zone.Contains(HandString);
        _onRightClick = _isHand
            ? battleState.AllowRightClickOnCard
                ? ToggleAsBasic
                : (Action)(() => { })
            : card.ShowDetailedCardView;
        controls.SetCanToggleBasic(_isHand && battleState.ShowSwapCardForBasic && card.Owner.BasicCard.IsPresent);
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
        _zone = LibraryString;
        _isHand = false;
        _requiresPlayerTargeting = false;
        RenderCardType();
    }

    private void LateUpdate()
    {
        if (!_isHand) 
            return;
        
        UpdateMouseDragStatus();
        
        if (_siblingIndex > -1)
        {
            if (IsFocused)
                transform.SetAsLastSibling();
            else
                transform.SetSiblingIndex(_siblingIndex);
        }
    }

    private void InitFreshCard(Action onClick)
    {
        gameObject.SetActive(true);
        targetRule.Hide();
        scalingRule.Hide();
        enemyTypePresenter.Hide();
        controls.SetActive(false);
        controls.SetCanToggleBasic(false);
        DisableCanPlayHighlight();
        DisableSelectedHighlight();
        fullScreenDarken.color = Transparent;
        _onClick = onClick;
        _onMiddleMouse = NoOp;
        _onRightClick = NoOp;
        _useCustomHoverActions = false;
        _hoverEnterAction = NoOp;
        _hoverExitAction = NoOp;
    }

    public void DisableInteractions()
    {
        _onClick = () => { };
        _onMiddleMouse = () => { };
        _onRightClick = () => { };
        _useCustomHoverActions = false;
        _hoverEnterAction = NoOp;
        _hoverExitAction = NoOp;
    } 
    
    private void NoOp() {}

    public void SetMiddleButtonAction(Action action) => _onMiddleMouse = action;

    public void SetHoverAction(Action enter, Action exit)
    {
        _hoverEnterAction = enter;
        _hoverExitAction = exit;
        _useCustomHoverActions = true;
    }

    public void ToggleAsBasic()
    {
        if (_card == null)
        {
            Log.Error("Developer Error - Only Card Instances can be used as Basics. This Card Presenter does not have a Card instance.");
            return;
        }
        if (_card.Owner.BasicCard.IsMissing)
            return;
        
        if (_debug)
            DebugLog($"UI - Toggle as Basic");
        _card.TransitionTo(_card.Mode != CardMode.Basic ? CardMode.Basic : CardMode.Normal);
        _cardType = _card.Type;
        _requiresPlayerTargeting = _cardType.RequiresPlayerTargeting();
        RenderCardType();
        ShowComprehensiveCardInfo();
        if (IsPlayable)
            SetHandHighlight(true);
        
        Message.Publish(new SwappedCard(transform, _card));
    }
    
    public void SetDisabled(bool isDisabled)
    {
        if (isDisabled)
        {
            if (_debug)
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

    public void SetSiblingIndex(int index) => _siblingIndex = index;

    private static readonly Color Transparent = new Color(0, 0, 0, 0);
    public void SetDetailHighlight(bool active)
    {
        if (!IsFocused && !active)
            return;
        
        if (_debug)
            DebugLog($"Setting Detail Highlight {active}");
        highlight.SetActive(active);
        IsFocused = active;
        SetShowComprehensiveInfo(active);
        fullScreenDarken.color = active ? Transparent : fullScreenDarkenFinalColor;
        fullScreenDarken.DOColor(active ? fullScreenDarkenFinalColor : Transparent, 0.6f);
    }

    public void SetHoverHighlight(bool active)
    {
        highlight.SetActive(active);
    }

    private const string HighlightString = "Highlight";
    private const string HighlightScaleString = "HighlightScale";
    
    public void SetHandHighlight(bool active)
    {
        if (!IsFocused && !active && AreCloseEnough(transform.localScale.x, 1.0f))
            return;

        if (_debug)
            DebugLog($"Setting Selected Highlight {active}");
        SetSelectedHighlight(IsPlayable && active);
        IsFocused = active;
        
        var showShowControls = active && _card.IsActive;
        controls.SetActive(showShowControls);
        
        SetShowComprehensiveInfo(active);

        var sign = active ? 1 : -1;
        var scale = active ? new Vector3(highlightedScale, highlightedScale, highlightedScale) : new Vector3(1f, 1f, 1f);
        var position = active ? _position + new Vector3(0, sign * 180f, sign * 2f) : _position;
        if (AreCloseEnough(scale.x, transform.localScale.x) && AreCloseEnough(position.y, transform.position.y))
            return;

        var tweenDuration = 0.08f;
        if (active)
        {
            Message.Publish(new TweenMovementRequested(transform, new Vector3(0, sign * 180f, sign * 2f), tweenDuration, MovementDimension.Spatial, TweenMovementType.RubberBand, HighlightString));
            Message.Publish(new TweenMovementRequested(transform, new Vector3(highlightedScale - 1f, highlightedScale - 1f, highlightedScale - 1f), tweenDuration, MovementDimension.Scale, TweenMovementType.RubberBand, HighlightScaleString));
        }
        else
        {
            Message.Publish(new SnapBackTweenRequested(transform, HighlightString));  
            Message.Publish(new SnapBackTweenRequested(transform, HighlightScaleString));  
        }
        
        if (_card != null)
            if (active)
                Message.Publish(new HighlightCardOwner(_card.Owner));
            else
                Message.Publish(new UnhighlightCardOwner(_card.Owner));
        
        if (active)
            Message.Publish(new CardHighlighted(this));
    }

    private void SetShowComprehensiveInfo(bool active)
    {
        if (active)
            ShowComprehensiveCardInfo();
        else
            HideComprehensiveCardInfo();
    }

    public void ShowComprehensiveCardInfo()
    {
        if (_debug)
            DebugLog("Show Comprehensive Info");
        Message.Publish(new HideReferencedCard());
        enemyTypePresenter.Hide();
        if (_card != null)
            rules.Show(_card,  _isHand ? 2 : 999);
        else
            rules.Show(_cardType, _isHand ? 2 : 999);
        targetRule.Show(_cardType.ActionSequences.First());
        if (!_isHand)
            scalingRule.Show(_cardType, _card?.Owner.PrimaryStat() ?? StatType.Power);

        var referencedCardParent2 = _isHand ? referencedCardParent2inHand : referencedCardParent2detailed;
        _cardType.ChainedCard.IfPresent(x => ShowReferencedCard(x, referencedCardParent1));
        _cardType.SwappedCard.IfPresent(x => ShowReferencedCard(x, _cardType.ChainedCard.IsPresent ? referencedCardParent2 : referencedCardParent1));
        var reactionCards = _cardType.BattleEffects().Where(x => x.IsReactionCard).ToArray();
        if (reactionCards.AnyNonAlloc() && (!_cardType.ChainedCard.IsPresent || !_cardType.SwappedCard.IsPresent)) 
            ShowReferencedCard(reactionCards[0].ReactionSequence, _cardType.ChainedCard.IsPresent || _cardType.SwappedCard.IsPresent ? referencedCardParent2 : referencedCardParent1);
        var chooseCardToCreateEffects = _card.BattleEffects().FirstOrDefault(x => x.EffectType == EffectType.ChooseCardToCreate);
        if (chooseCardToCreateEffects != null && chooseCardToCreateEffects.EffectScope.Value.Count(x => x == ',') == 1)
        {
            var choiceCardIds = chooseCardToCreateEffects.EffectScope.Value.Split(',').Select(int.Parse).ToArray();
            Message.Publish(new ShowReferencedCard(referencedCardParent1, new Card(battleState.GetNextCardId(), _card.Owner, allCards.GetCardById(choiceCardIds[0]).Value, _card.OwnerTint, _card.OwnerBust)));
            Message.Publish(new ShowReferencedCard(referencedCardParent2, new Card(battleState.GetNextCardId(), _card.Owner, allCards.GetCardById(choiceCardIds[1]).Value, _card.OwnerTint, _card.OwnerBust)));
        }
    }

    private void ShowReferencedCard(CardTypeData c, GameObject referenceCardParent)
    {
        if (_card != null)
            Message.Publish(new ShowReferencedCard(referenceCardParent, new Card(-1, _card.Owner, c, _card.OwnerTint, _card.OwnerBust)));
        else
            Message.Publish(new ShowReferencedCard(referenceCardParent, c));
    }
    
    private void HideComprehensiveCardInfo()
    {
        rules.Hide();
        targetRule.Hide();
        scalingRule.Hide();
        enemyTypePresenter.Show();
        Message.Publish(new HideReferencedCard());
    }
    
    public void TeleportTo(Vector3 targetPosition)
    {
        Message.Publish(new StopMovementTweeningRequested(transform, MovementDimension.Spatial));
        transform.position = targetPosition;
        _position = targetPosition;
    }
    
    public void SetTargetPosition(Vector3 targetPosition)
    {
        if (_debug)
            DebugLog($"Set Target Position {targetPosition.ToString()}");
        _position = targetPosition;
        Message.Publish(new GoToTweenRequested(transform, targetPosition, Vector3.Distance(transform.position, targetPosition) / 1600f, MovementDimension.Spatial));
    }

    private void RenderCardType()
    {
        var shouldUseLibraryMode = _card == null || _card.Owner.TeamType == TeamType.Party && (_card.Cost.PlusXCost && !_isHand);
        IsPlayable = CheckIfCanPlay();
        nameLabel.SetTerm(_cardType.NameTerm);

        description.SetFinalText(shouldUseLibraryMode
            ? _cardType.LocalizedDescription(_card?.Owner ?? Maybe<Member>.Missing(), ResourceQuantity.DontInterpolateX)
            : _cardType.LocalizedDescription(_card.Owner, _card.LockedXValue.OrDefault(() => _card.Owner.CalculateResources(_card.Type).XAmountQuantity), handZone.Count, state.PlayerState.NumberOfCyclesUsedThisTurn));

        typeLabel.SetFinalText(_cardType.LocalizedArchetypeDescription());
        art.sprite = _cardType.Art;
        rarity.Set(_cardType.Rarity);
        target.Set(_cardType);
        cardCostPresenter.Render(shouldUseLibraryMode ? Maybe<Card>.Missing() : new Maybe<Card>(_card, _card != null), _cardType, 
            _card != null ? _card.Owner.State.PrimaryResource : _cardType.Cost.ResourceType);
        if (_card != null && _card.OwnerBust.IsPresent)
            bust.Show(_card.OwnerBust.Value);
        else
            bust.Hide();
        SetCardTint();
        UpdateCardHighlight();
        if (_card == null || _card.Mode != CardMode.Glitched)
            glitchableComponents.ForEach(x => x.material = null);
        else 
            glitchableComponents.ForEach(x => x.material = glitchMaterial);
        dragRotator.Reset();
        dragRotator.enabled = _isHand;
        onlyEnabledInHand.ForEach(o => o.SetActive(_isHand));
        background.sprite = _card?.IsSinglePlay ?? _cardType.IsSinglePlay ? transientCard : standardCard;
    }

    private void UpdateCardHighlight()
    {
        SetCanPlayHighlight(IsPlayable,
            _card != null
                ? _cardType.HighlightCondition.Map(condition =>
                    condition.ConditionMet(new CardConditionContext(_card, battleState)))
                : Maybe<bool>.Missing(),
            _card != null
                ? _cardType.UnhighlightCondition.Map(condition =>
                    condition.ConditionMet(new CardConditionContext(_card, battleState)))
                : Maybe<bool>.Missing());
    }

    private void SetCardTint()
    {
        tint.color = _card == null
            ? Color.white
            : _card.OwnerTint.OrDefault(Color.white);
        tintGradient.enabled = _card == null || _card.OwnerTint.IsMissing;
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
        if (_debug)
            DebugLog($"Can Play: {result}");
        return result;
    }

    public void ShowEnemyCardType(string typeTerm) => enemyTypePresenter.Init(typeTerm);

    private void DebugLog(string msg)
    {
        if (_debug)
            Log.Info($"Card {CardName}: {msg}");
    }
    
    #region Mouse Controls

    private bool _buttonAlreadyDown => _leftButtonAlreadyDown || _rightButtonAlreadyDown;
    private bool _leftButtonAlreadyDown = false;
    private bool _rightButtonAlreadyDown = false;
    public void MiddleClick() => _onMiddleMouse();
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (MouseDragState.IsDragging || eventData.dragging || _buttonAlreadyDown)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
            _leftButtonAlreadyDown = true;
        if (eventData.button == PointerEventData.InputButton.Right)
            _rightButtonAlreadyDown = true;
        if (_debug)
            DebugLog($"UI - Pointer Down");
        if (_isHand && CheckIfCanPlay() && eventData.button == PointerEventData.InputButton.Left)
        {
            Message.Publish(new CardClicked());
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

    private const string ClickString = "Click";
    public void OnPointerUp(PointerEventData eventData)
    {
        if (MouseDragState.IsDragging && _buttonAlreadyDown == false) 
            return;
        
        if (_debug)
            DebugLog("UI - Pointer Up");
        if (_leftButtonAlreadyDown && eventData.button == PointerEventData.InputButton.Left)
        {
            Cursor.visible = true;
            _leftButtonAlreadyDown = false; 
            if (_isHand && IsPlayable)
                Message.Publish(new SnapBackTweenRequested(transform, ClickString));
        }
        if (_rightButtonAlreadyDown && eventData.button == PointerEventData.InputButton.Right)
        {
            Cursor.visible = true;
            _rightButtonAlreadyDown = false;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!MouseDragState.IsDragging && _isHand && battleState.Phase == BattleV2Phase.PlayCards && Vector3.Distance(_position, transform.position) <= 0.1)
        {
            Message.Publish(new CardHoverEnter(this));
            Message.Publish(new CardHoverSFX(transform));
        }
        else if (_useCustomHoverActions)
        {
            _hoverEnterAction();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!MouseDragState.IsDragging && !IsDragging && _isHand)
        {
            SetHandHighlight(false);
            Message.Publish(new CardHoverExitSFX(transform));
        }
        else if (_useCustomHoverActions)
        {
            _hoverExitAction();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsDragging)
            eventData.pointerDrag = null;
        else
            WhenActivatableHand(() =>
            {
                if (!_requiresPlayerTargeting || !IsPlayable)
                {
                    var targetPoint = transform.position + new Vector3(eventData.delta.x * dragScaleFactor,
                        eventData.delta.y * dragScaleFactor, 0);
                    var clampedDragPoint = new Vector3(
                        Mathf.Clamp(targetPoint.x, _minDragPoint.x, _maxDragPoint.x),
                        Mathf.Clamp(targetPoint.y, _minDragPoint.y, _maxDragPoint.y),
                        targetPoint.z);
                    transform.position = clampedDragPoint;
                    eventData.position = clampedDragPoint;
                }
            }, () => { });
    }

    
    private void UpdateMouseDragStatus()
    {
        if (!_isHand || !MouseDragState.IsDragging)
            return;
        
        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
            CleanupOnDragEnded();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
        => WhenActivatableHand(() =>
        {
            Cursor.visible = false;
            IsDragging = true;
            MouseDragState.Set(true);
            controls.SetActive(false);
            canvasGroup.blocksRaycasts = false;
            HideComprehensiveCardInfo();

            if (_rightButtonAlreadyDown == true && _card.Mode == CardMode.Normal)
                _card.TransitionTo(CardMode.Basic);

            // Targeting Card Selection Process can run the arrow
            if (_requiresPlayerTargeting && IsPlayable)
                Message.Publish(new ShowMouseTargetArrow(transform));

            // This is crude. Reason for not being able to play a card should flow through
            var owner = _card.Owner;
            if (owner.IsDisabled())
                Message.Publish(new ShowHeroBattleThought(owner.Id, "I'm disabled this turn. I can only discard, watch an ally play a card, or end the turn early."));
            else if (_card.Cost.BaseAmount > _card.Owner.ResourceAmount(_card.Cost.ResourceType))
                Message.Publish(new ShowHeroBattleThought(owner.Id, $"I don't have enough {_card.Cost.ResourceType.Name} to play this card right now."));
            else if (conditionNotMetHighlight.activeSelf && _card.UnhighlightCondition is { IsPresent: true })
                Message.Publish(new ShowHeroBattleThought(owner.Id, _card.UnhighlightCondition.Value.UnhighlightMessage));

            _onBeginDrag();
            Message.Publish(new CardDragged());
            Message.Publish(new BeginTargetSelectionRequested(_card));
        }, () => { });

    public void OnEndDrag(PointerEventData eventData) 
        => CleanupOnDragEnded();

    private void CleanupOnDragEnded()
    {
        WhenActivatableHand(() =>
        {
            MouseDragState.Set(false);
            Message.Publish(new CancelTargetSelectionRequested());
            ReturnHandToNormal();
        }, UndoDragMovement);
    }

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
        MouseDragState.Set(false);
    }
    
    private void ReturnHandToNormal()
    {
        IsDragging = false;
        canvasGroup.blocksRaycasts = true;
        Message.Publish(new HideMouseTargetArrow());
        if (_card != null)
            Message.Publish(new UnhighlightCardOwner(_card.Owner));
    }

    public void Cancel() => ReturnHandToNormal();
    
    public void Discard()
    {
        if (battleState.NumberOfCardPlaysRemainingThisTurn <= 0)
            return;
        if (_debug)
            DebugLog($"Discard");
        ReturnHandToNormal();
        Message.Publish(new CardDiscarded(transform, _card));
        _onDiscard();
    }
    
    public void Activate()
    {
        if (_debug)
            DebugLog($"Activate");
        ReturnHandToNormal();
        _onClick();
    }

    #endregion
}
