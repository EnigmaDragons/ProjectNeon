using System;
using System.Linq;
using UnityEngine;

public class CardsVisualizer : MonoBehaviour
{
    [SerializeField] private CardPlayZone zone;
    [SerializeField] private CardPlayZone onCardClickDestination;
    [SerializeField] private float cardSpacingScreenPercent = 0.15f;
    [SerializeField] private CardPresenter cardPrototype;

    [ReadOnly] [SerializeField] private GameObject[] _shownCards = new GameObject[0];
    private bool _isDirty = false;
    private Action _onShownCardsChanged = () => { };

    public GameObject[] ShownCards => _shownCards;

    public void SetOnShownCardsChanged(Action action) => _onShownCardsChanged = action;
    
    void OnEnable()
    {
        zone.OnZoneCardsChanged.Subscribe(
            new GameEventSubscription(zone.OnZoneCardsChanged.name, x => _isDirty = true, this));
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
        KillPreviousCards();
        CreateCurrentCards(zone.Cards.ToArray());
        _onShownCardsChanged();
    }
    
    // @todo #30:30min Animate these cards entrances. Should slide in from right of screen

    private void CreateCurrentCards(Card[] cards)
    {
        var newShownCards = new GameObject[cards.Length];
        var totalSpaceNeeded = Screen.width * (cardSpacingScreenPercent * cards.Length);
        var startX = (Screen.width - totalSpaceNeeded) / 2f;
        for (var i = 0; i < cards.Length; i++)
        {
            var cardIndex = i;
            var c = Instantiate(cardPrototype, 
                new Vector3(startX + cardSpacingScreenPercent * (i + 0.5f) * Screen.width, transform.position.y, transform.position.z), 
                cardPrototype.transform.rotation, 
                gameObject.transform);
            c.Set(cards[cardIndex], () => SelectCard(cardIndex));
            newShownCards[cardIndex] = c.gameObject;
        }
        _shownCards = newShownCards;
    }

    private void KillPreviousCards()
    {
        var shown = _shownCards.ToArray();
        shown.ForEach(x => DestroyImmediate(x));
    }

    public void SelectCard(int cardIndex)
    {
        onCardClickDestination.PutOnBottom(zone.Take(cardIndex));
    }
}
