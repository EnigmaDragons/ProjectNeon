using System.Linq;
using UnityEngine;

/**
 * Deck visualizer: shows the deck in a list format in  DeckBuilder screen, according
 * to the selected Hero.
 */
public class DeckVisualizer : MonoBehaviour
{
    /**
     * Deck builder state
     */
    [SerializeField] private DeckBuilderState state;

    /**
     * Card list spacing, relative to screen height
     */
    [SerializeField] private float cardSpacingScreenPercent = 0.15f;

    /**
     * Cards shown in screen
     */
    [ReadOnly] [SerializeField] private GameObject[] _shownCards = new GameObject[0];
    public GameObject[] ShownCards => _shownCards;

    /**
     * Indicates if some change happened in the DeckVisualizer (i.e, it's dirty)
     */
    private bool _isDirty = false;

    [SerializeField] private CardListItem cardPrototype;

    /**
     * Updates the list
     */
    public void Update()
    {
        if (!_isDirty)
            return;

        _isDirty = false;
        UpdateDeckListView(this.state.Current());
    }


    /**
     * Updates the DeckListView
     */
    private void UpdateDeckListView(Deck deck)
    {
        Clear();
        CreateCurrentCards(deck.Cards.ToArray());
    }

    /**
     * Create the current list object for a set of cards
     */
    private void CreateCurrentCards(Card[] cards)
    {
        var newShownCards = new GameObject[cards.Length];
        var totalSpaceNeeded = Screen.height * (cardSpacingScreenPercent * cards.Length);
        var startY = (Screen.height - totalSpaceNeeded) / 2f;
        for (var i = 0; i < cards.Length; i++)
        {
            var cardIndex = i;
            var c = Instantiate(cardPrototype,
                new Vector3(transform.position.x, startY + cardSpacingScreenPercent * (i + 0.5f) * Screen.height, transform.position.z),
                cardPrototype.transform.rotation,
                gameObject.transform);
            c.Set(cards[cardIndex], () => SelectCard(cardIndex));
            newShownCards[cardIndex] = c.gameObject;
        }
        _shownCards = newShownCards;

    }

    public void SelectCard(int cardIndex)
    {
        /**
         * @todo #216:30min Create select card logic. When selecting a card, it should get into the 
         *  current deck area
         */
    }

    /**
     * Clear list
     */
    private void Clear()
    {
        var shown = _shownCards.ToArray();
        shown.ForEach(x => DestroyImmediate(x));
    }

    void OnEnable()
    {
        this._isDirty = true;
        state.OnCurrentDeckChanged.Subscribe(
            new GameEventSubscription(state.OnCurrentDeckChanged.name, x => _isDirty = true, this));
    }

    void OnDisable()
    {
        state.OnCurrentDeckChanged.Unsubscribe(this);
    }
}
