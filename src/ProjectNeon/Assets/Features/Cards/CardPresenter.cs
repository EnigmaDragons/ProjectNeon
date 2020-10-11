﻿using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
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
    [SerializeField] private GameObject controls;
    [SerializeField] private float highlightedScale = 1.7f;

    private Card _card;
    private CardTypeData _cardType;

    private Func<BattleState, Card, bool> _getCanPlay;
    private Action _onClick;
    private Action _onMiddleMouse;
    private Vector3 _position;
    private bool _isDragging;

    public string CardName => _cardType.Name;
    public bool Contains(Card c) => HasCard && c.Id == _card.Id;
    public bool Contains(CardTypeData c) => HasCard && _cardType.Name.Equals(c.Name);
    public bool HasCard => _cardType != null;
    public bool IsHighlighted => highlight.activeSelf;
    public bool IsPlayable { get; private set; }
    
    private void Update()
    {
        if (!_isDragging)
            return;
    }
    
    public void Clear()
    {
        gameObject.SetActive(false);
        _card = null;
        _cardType = null;
    }
    
    public void Set(Card card, Action onClick, Func<BattleState, Card, bool> getCanPlay)
    {
        InitFreshCard(onClick);

        _card = card;
        _cardType = card.Type;
        _getCanPlay = getCanPlay;
        RenderCardType();
    }
    
    public void Set(CardTypeData cardType, Action onClick)
    {
        InitFreshCard(onClick);

        _card = null;
        _cardType = cardType;
        _getCanPlay = (_, __) => false;
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
        
        Debug.Log($"UI - Toggle as Basic {CardName}");
        
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
            Debug.Log($"{CardName} is disabled.");
            canPlayHighlight.SetActive(false);
            controls.SetActive(false);
        }
        darken.SetActive(isDisabled);
    }
    
    public void SetHandHighlight(bool active)
    {
        if (!highlight.activeSelf && !active && AreCloseEnough(transform.localScale.x, 1.0f))
            return;

        Debug.Log($"Setting Highlight {active}");
        controls.SetActive(active);
        if (active)
            transform.SetAsLastSibling();
        highlight.SetActive(IsPlayable && active);
        var sign = active ? 1 : -1;
        var scale = active ? new Vector3(highlightedScale, highlightedScale, highlightedScale) : new Vector3(1f, 1f, 1f);
        var position = active ? _position + new Vector3(0, sign * 100f, sign * 2f) : _position;
        if (AreCloseEnough(scale.x, transform.localScale.x) && AreCloseEnough(position.y, transform.position.y))
            return;
        
        transform.DOScale(scale, 0.4f);
        transform.DOMove(position, 0.4f);
        if (active && _card != null)
            Message.Publish(new HighlightCardOwner(_card.Owner));
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
            ? _cardType.InterpolatedDescription(_card.Owner) 
            : _cardType.InterpolatedDescription(Maybe<Member>.Missing());
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

        _cardType.LimitedToClass.IfPresent(c => tint.color = c.Tint);
        canPlayHighlight.SetActive(IsPlayable);
        highlight.SetActive(IsPlayable);
    }
    
    private bool AreCloseEnough(float first, float second) => WithinEpsilon(first - second);
    private bool WithinEpsilon(float f) => Math.Abs(f) < 0.05;

    private bool CheckIfCanPlay()
    {
        var result = _card != null && _getCanPlay(battleState, _card);
        Debug.Log($"Can Play {CardName}: {result}");
        return result;
    }

    #region Mouse Controls
    public void OnPointerDown(PointerEventData eventData)
    {
        Log.Info($"UI - Pointer Down - {CardName}");
        if (battleState.SelectionStarted)
            return;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Log.Info($"UI - Clicked {CardName}");
            _onClick();
        }
        
        if (eventData.button == PointerEventData.InputButton.Middle) 
            _onMiddleMouse();
        if (_card != null && eventData.button == PointerEventData.InputButton.Right)
            ToggleAsBasic();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsPlayable)
            return;
        
        Message.Publish(new CardHoverEnter(this));
    }

    public void OnPointerExit(PointerEventData eventData) => SetHandHighlight(false);

    #endregion
}
