using System;
using System.Linq;
using UnityEngine;

public class CardsVisualizer : MonoBehaviour
{
    [SerializeField] private BattleState state;
    [SerializeField] private CardPlayZone zone;
    [SerializeField] private bool allowInteractions = true;
    [SerializeField] private CardPlayZone onCardClickDestination;
    [SerializeField] private float cardSpacingScreenPercent = 0.15f;
    [SerializeField] private CardPresenter cardPrototype;
    [SerializeField] private int maxCards = 12;
    [SerializeField] private bool onlyAllowInteractingWithPlayables = false;
    [SerializeField] private Vector3 cardRotation;
    
    [ReadOnly] [SerializeField] private CardPresenter[] cardPool;
    
    private Card[] _oldCards = new Card[0];
    private bool _isDirty = false;
    private Action _onShownCardsChanged = () => { };
    private bool _isFocused = true;

    private void Awake()
    {
        cardPool = new CardPresenter[maxCards];
        for (var i = 0; i < maxCards; i++)
        {
            cardPool[i] = Instantiate(cardPrototype, transform);
            cardPool[i].Clear();
            cardPool[i].transform.rotation = Quaternion.Euler(cardRotation);
        }
    }
    
    void OnEnable()
    {
        _isDirty = true;
        zone.OnZoneCardsChanged.Subscribe(new GameEventSubscription(zone.OnZoneCardsChanged.name, x => _isDirty = true, this));
        Message.Subscribe<MemberStateChanged>(_ => _isDirty = true, this);
    }

    void OnDisable()
    {
        zone.OnZoneCardsChanged.Unsubscribe(this);
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
        var newCards = zone.Cards.ToArray();
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

            foreach (var c in cardPool)
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
            var effectivePosition = transform.position;
            var cardIndex = i;
            var card = cards[cardIndex];
            var (presenterIndex, presenter) = GetCardPresenter(cardIndex, card);
            var c = presenter;
            var isHighlighted = c.IsHighlighted;
            
            if (!c.HasCard)
                c.MoveTo(new Vector3(screenWidth * 1.5f, effectivePosition.y, effectivePosition.z));
            
            var targetX = startX + cardSpacingScreenPercent * (cardIndex + 0.5f) * screenWidth;
            var targetPosition = new Vector3(targetX, effectivePosition.y, effectivePosition.z);

            c.Set(card, () => SelectCard(cardIndex));
            c.SetCanPlay(allowInteractions && (!onlyAllowInteractingWithPlayables || card.IsPlayableByHero(state)));
            c.SetDisabled(!_isFocused);
            if (!card.Owner.IsConscious() || card.Owner.IsStunnedForCurrentTurn())
                c.SetDisabled(true);
            SwapCardPoolSpots(cardIndex, presenterIndex);
            c.SetHighlight(isHighlighted);
            c.SetTargetPosition(targetPosition);
        }
    }

    private void SwapCardPoolSpots(int first, int second)
    {
        if (first == second)
            return;
        
        var tmp = cardPool[first];
        cardPool[first] = cardPool[second];
        cardPool[second] = tmp;
    }
    
    private (int, CardPresenter) GetCardPresenter(int startAtIndex, Card c)
    {
        CardPresenter emptyCard = null;
        var emptyCardIndex = -1;
        
        for (var i = startAtIndex; i < cardPool.Length; i++)
        {
            var cp = cardPool[i];
            
            // Find First Unused Card Presenter
            if (emptyCard == null && !cp.HasCard)
            {
                emptyCard = cp;
                emptyCardIndex = i;
            }

            // Return Matching Card
            if (cp.Contains(c))
                return (i, cp);
        }

        return (emptyCardIndex, emptyCard);
    }
    
    public void SelectCard(int cardIndex)
    {
        if (allowInteractions)
            if (cardPool[cardIndex].IsPlayable || !onlyAllowInteractingWithPlayables)
                onCardClickDestination.PutOnBottom(zone.Take(cardIndex));
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
        UpdateCurrentCards(_oldCards);
    }
}
