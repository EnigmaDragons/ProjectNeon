using System;
using System.Linq;
using UnityEngine;

public sealed class HandVisualizer : MonoBehaviour
{
    [SerializeField] private BattleState state;
    [SerializeField] private CardPlayZones zones;
    [SerializeField] private bool allowInteractions = true;
    [SerializeField] private float cardSpacingScreenPercent = 0.15f;
    [SerializeField] private CardPresenter cardPrototype;
    [SerializeField] private int maxCards = 12;
    [SerializeField] private bool onlyAllowInteractingWithPlayables = false;
    [SerializeField] private Vector3 unfocusedOffset = new Vector3(0, 400, 0);
    [SerializeField] private Vector3 cardRotation;

    private CardPlayZone Hand => zones.HandZone;
    private CardPool _cardPool;
    private Card[] _oldCards = new Card[0];
    private bool _isDirty = false;
    private Action _onShownCardsChanged = () => { };
    private Vector3 _defaultPosition;
    private Vector3 _unfocusedPosition;
    private bool _isFocused = true;
    private bool _useRecycle = false;

    public CardPresenter[] ShownCards => _cardPool.ShownCards;

    public void SetOnShownCardsChanged(Action action) => _onShownCardsChanged = action;

    public void ReProcess() => UpdateCurrentCards(Hand.Cards.ToArray());

    private void Awake()
    {
        _defaultPosition = transform.position;
        _unfocusedPosition = _defaultPosition - unfocusedOffset;
        _cardPool = new CardPool(maxCards, this, cardPrototype, cardRotation);
    }
    
    void OnEnable()
    {
        _isDirty = true;
        Hand.OnZoneCardsChanged.Subscribe(new GameEventSubscription(Hand.OnZoneCardsChanged.name, x => _isDirty = true, this));
        Message.Subscribe<MemberStateChanged>(_ => _isDirty = true, this);
    }

    void OnDisable()
    {
        Hand.OnZoneCardsChanged.Unsubscribe(this);
        Message.Unsubscribe(this);
    }

    void Update()
    {
        if (!_isDirty)
            return;

        _isDirty = false;
        UpdateVisibleCards();
    }
    
    void UpdateVisibleCards()
    {
        var newCards = Hand.Cards.ToArray();
        CleanRemovedCards(newCards);
        UpdateCurrentCards(newCards);
        _oldCards = newCards;
        _onShownCardsChanged();
    }

    private void CleanRemovedCards(Card[] newCards)
    {
        var newCopy = newCards.ToList();
        foreach (var old in _oldCards)
        {
            if (newCopy.Contains(old))
            {
                newCopy.Remove(old);
                continue;
            }

            foreach (var c in _cardPool.ShownCards)
            {
                if (c.Contains(old))
                {
                    c.Clear();
                    c.transform.position += new Vector3(1920, 0, 0);
                    break;
                }
            }
        }
    }

    private void UpdateCurrentCards(Card[] cards)
    {
        var screenWidth = Screen.width;
        var totalSpaceNeeded = screenWidth * (cardSpacingScreenPercent * cards.Length);
        var startX = (screenWidth - totalSpaceNeeded) / 2f;

        var highlightedCardIndex = -1;
        for (var i = 0; i < cards.Length; i++)
        {
            var effectivePosition = _isFocused ? _defaultPosition : _unfocusedPosition;
            var cardIndex = i;
            var card = cards[cardIndex];
            var (presenterIndex, presenter) = _cardPool.GetCardPresenter(cardIndex, card);
            var c = presenter;
            var isHighlighted = c.IsHighlighted;
            if (isHighlighted)
                highlightedCardIndex = presenterIndex;
            
            if (!c.HasCard)
                c.MoveTo(new Vector3(screenWidth * 1.5f, effectivePosition.y, effectivePosition.z));
            
            var targetX = startX + cardSpacingScreenPercent * (cardIndex + 0.5f) * screenWidth;
            var targetPosition = new Vector3(targetX, effectivePosition.y, effectivePosition.z);

            c.Set(card, () => SelectCard(cardIndex), true);
            c.SetCanPlay(allowInteractions && (!onlyAllowInteractingWithPlayables || card.IsPlayableByHero(state)));
            c.SetDisabled(!_isFocused);
            if (!card.Owner.CanPlayCards())
                c.SetDisabled(true);
            _cardPool.SwapItems(cardIndex, presenterIndex);
            c.SetHighlight(isHighlighted);
            c.SetTargetPosition(targetPosition);
            c.SetMiddleButtonAction(() => RecycleCard(cardIndex));
            c.transform.SetAsLastSibling();
        }
        
        if (highlightedCardIndex > -1)
            _cardPool.ShownCards[highlightedCardIndex].transform.SetAsLastSibling();

        if (cards.Any() && state.NumberOfRecyclesRemainingThisTurn == 0 && cards.All(c => !c.IsAnyFormPlayableByHero() || !c.Owner.CanPlayCards()))
        {
            BattleLog.Write("No playable cards. Requesting early turn Confirmation.");
            Message.Publish(new BeginPlayerTurnConfirmation());
        }
    }
    
    public void SelectCard(int cardIndex)
    {
        if (allowInteractions && Hand.Count > cardIndex)
            if (_cardPool[cardIndex].IsPlayable || !onlyAllowInteractingWithPlayables)
            {
                zones.SelectionZone.PutOnBottom(Hand.Take(cardIndex));
                UpdateVisibleCards();
            }
    }

    public void RecycleCard(int cardIndex)
    {
        if (state.NumberOfRecyclesRemainingThisTurn < 1)
            return;
        
        if (zones.DrawZone.Count < 1)
            zones.Reshuffle();

        state.UseRecycle();
        zones.DiscardZone.PutOnBottom(Hand.Take(cardIndex).RevertedToStandard());
        Hand.PutOnBottom(zones.DrawZone.DrawOneCard());
    }

    public void SetFocus(bool isFocused)
    {
        if (_isFocused == isFocused)
            return;
        
        _isFocused = isFocused;
        _isDirty = true;
    }

    public void RefreshPositions()
    {
        _defaultPosition = transform.position;
        _unfocusedPosition = _defaultPosition - unfocusedOffset;
        UpdateCurrentCards(_oldCards);
    }
}
