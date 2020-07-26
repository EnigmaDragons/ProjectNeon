using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class CardsVisualizer : MonoBehaviour
{
    [SerializeField] private CardPlayZone zone;
    [SerializeField] private bool allowInteractions = true;
    [SerializeField] private CardPlayZone onCardClickDestination;
    [SerializeField] private float cardSpacingScreenPercent = 0.15f;
    [SerializeField] private CardPresenter cardPrototype;
    [SerializeField] private int maxCards = 12;
    
    [ReadOnly] [SerializeField] private CardPresenter[] cardPool;
    private Card[] _oldCards = new Card[0];
    private bool _isDirty = false;
    private Action _onShownCardsChanged = () => { };

    public GameObject[] ShownCards => cardPool.Select(c => c.gameObject).ToArray();

    public void SetOnShownCardsChanged(Action action) => _onShownCardsChanged = action;

    private void Awake()
    {
        cardPool = new CardPresenter[maxCards];
        for (var i = 0; i < maxCards; i++)
        {
            cardPool[i] = Instantiate(cardPrototype, transform);
            cardPool[i].Clear();
        }
    }
    
    void OnEnable()
    {
        _isDirty = true;
        zone.OnZoneCardsChanged.Subscribe(new GameEventSubscription(zone.OnZoneCardsChanged.name, x => _isDirty = true, this));
    }

    void OnDisable()
    {
        zone.OnZoneCardsChanged.Unsubscribe(this);
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
                    Debug.Log($"Cleaned Old Card {old}");
                    c.Clear();
                    c.transform.position += new Vector3(1920, 0, 0);
                    break;
                }
            }
        }
    }

    private void UpdateCurrentCards(Card[] cards)
    {
        var totalSpaceNeeded = Screen.width * (cardSpacingScreenPercent * cards.Length);
        var startX = (Screen.width - totalSpaceNeeded) / 2f;

        for (var i = 0; i < cards.Length; i++)
        {
            var cardIndex = i;
            var card = cards[cardIndex];
            var (presenterIndex, presenter) = GetCardPresenter(cardIndex, card);
            var c = presenter;
            
            if (!c.HasCard)
                c.transform.position = new Vector3(1920, transform.position.y, transform.position.z);
            
            var targetX = startX + cardSpacingScreenPercent * (cardIndex + 0.5f) * Screen.width;
            var targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);

            c.Set(card, () => SelectCard(cardIndex));
            SwapCardPoolSpots(cardIndex, presenterIndex);
            c.transform.DOMove(targetPosition, 1);
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
            onCardClickDestination.PutOnBottom(zone.Take(cardIndex));
    }
}
