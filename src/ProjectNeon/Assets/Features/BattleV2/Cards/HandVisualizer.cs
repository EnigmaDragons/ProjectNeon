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
    [SerializeField] private Vector3 unfocusedOffset = new Vector3(0, 400, 0);
    [SerializeField] private Vector3 cardRotation;

    private CardPlayZone Hand => zones.HandZone;
    private CardPool _cardPool;
    private Card[] _oldCards = new Card[0];
    private bool _isDirty = false;
    private Action _onShownCardsChanged = () => { };
    private Vector3 _defaultPosition;
    private Vector3 _unfocusedPosition;
    private bool _useRecycle = false;
    private bool _cardPlayingAllowed = true;

    public CardPresenter[] ShownCards => _cardPool == null ? new CardPresenter[0] : _cardPool?.ShownCards;

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
        Log.Info($"UI - Hand Enabled");
        Hand.OnZoneCardsChanged.Subscribe(new GameEventSubscription(Hand.OnZoneCardsChanged.name, x => _isDirty = true, this));
        Message.Subscribe<MemberStateChanged>(_ => _isDirty = true, this);
        Message.Subscribe<PlayerCardCanceled>(_ => _isDirty = true, this);
        Message.Subscribe<CancelTargetSelectionRequested>(_ => CancelCardPlays(), this);
    }

    void OnDisable()
    {
        Log.Info($"UI - Hand Disabled");
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

    private void CancelCardPlays()
    {
        ShownCards.ForEach(c => c.Cancel());
    }
    
    public void SetCardPlayingAllowed(bool isAllowed)
    {
        allowInteractions = isAllowed;
        UpdateVisibleCards();
    }

    public void UpdateVisibleCards()
    {
        var newCards = Hand.Cards.ToArray();
        CleanRemovedCards(newCards);
        UpdateCurrentCards(newCards);
        _oldCards = newCards;
        _onShownCardsChanged();
    }

    private void CleanRemovedCards(Card[] newCards)
    {
        #if UNITY_EDITOR
        if (_cardPool == null)
            return;
        #endif
        
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

        for (var i = 0; i < cards.Length; i++)
        {
            var effectivePosition = _defaultPosition;
            var cardIndex = i;
            var card = cards[cardIndex];
            var (presenterIndex, presenter) = _cardPool.GetCardPresenter(cardIndex, card);
            var c = presenter;
            var isFocused = c.IsFocused;
            
            if (!c.HasCard)
                c.TeleportTo(new Vector3(screenWidth * 1.5f, effectivePosition.y, effectivePosition.z));
            
            var targetX = startX + cardSpacingScreenPercent * (cardIndex + 0.5f) * screenWidth;
            var targetPosition = new Vector3(targetX, effectivePosition.y, effectivePosition.z);
            _cardPool.SwapItems(cardIndex, presenterIndex);

            if (c.IsDragging) 
                continue;
            
            c.Set("Hand", card,
                () => SelectCard(cardIndex),
                () => BeginDragCard(card, cardIndex),
                () => DiscardCard(cardIndex),
                (battleState, c2) => allowInteractions && c2.IsPlayable(battleState.Party, battleState.NumberOfCardPlaysRemainingThisTurn),
                () => allowInteractions);
            c.SetSiblingIndex(cardIndex);
            c.SetMiddleButtonAction(() => RecycleCard(cardIndex));
            c.SetDisabled(card.Owner.IsUnconscious() || card.Owner.IsDisabled());
            c.SetHandHighlight(isFocused);
            c.SetTargetPosition(targetPosition);
        }
    }

    private void DiscardCard(int cardIndex)
    {
        if (state.Phase != BattleV2Phase.PlayCards)
            return;
        
        if (allowInteractions && Hand.Count > cardIndex)
            Message.Publish(new EndTargetSelectionRequested(true));
        else
            Message.Publish(new CancelTargetSelectionRequested());
    }
    
    public void SelectCard(int cardIndex)
    {
        if (state.Phase != BattleV2Phase.PlayCards)
            return;
        
        if (allowInteractions && Hand.Count > cardIndex)
            Message.Publish(new EndTargetSelectionRequested(false));
        else
            Message.Publish(new CancelTargetSelectionRequested());
    }

    public void BeginDragCard(Card card, int cardIndex)
    {
        if (state.Phase != BattleV2Phase.PlayCards)
            return;
        
        if (card.Type.RequiresPlayerTargeting() && _cardPool[cardIndex].IsPlayable)
        {
            var targetPosition = new Vector3(Screen.width / 2f, _defaultPosition.y, _defaultPosition.z);
            _cardPool[cardIndex].SetTargetPosition(targetPosition);
        }
    }

    public void RecycleCard(int cardIndex)
    {
        if (state.NumberOfRecyclesRemainingThisTurn < 1)
            return;
        
        state.UseRecycle();
        zones.DiscardZone.PutOnBottom(Hand.Take(cardIndex).RevertedToStandard());
        zones.DrawOneCard();
        Message.Publish(new CheckForAutomaticTurnEnd());
    }
}
