using System.Linq;
using UnityEngine;

public class HandViewVisualizer : MonoBehaviour
{
    [SerializeField] private CardPlayZone cardsInHand;
    [SerializeField] private float minX = 200;
    [SerializeField] private float maxX = 1920 - 200;
    [SerializeField] private float spaceBetweenCards = 200;
    [SerializeField] private GameObject cardPrototype;

    private GameObject[] _shownCards = new GameObject[0];

    void Awake()
    {
        cardsInHand.OnZoneCardsChanged.Subscribe(
            new GameEventSubscription(cardsInHand.OnZoneCardsChanged.name, x => UpdateVisibleCards(), this));
    }

    void OnDisable()
    {
        cardsInHand.OnZoneCardsChanged.Unsubscribe(this);
    }

    // @todo #30:15min This isn't parallel-safe. Too many cards are created during setup. Implement intelligent card diffs.

    // @todo #30:30min Animate these cards entrances. Should slide in from right of screen

    // @todo #30:15min Space card out from the center, instead of from Left of Zone, and add a little tilt, based on card index.

    void UpdateVisibleCards()
    {
        var shown = _shownCards.ToArray();
        shown.ForEach(x => Destroy(x));
        var cards = cardsInHand.Cards.ToArray();
        for (var i = 0; i < cards.Length; i++)
            Instantiate(cardPrototype, new Vector3(minX + spaceBetweenCards * i, transform.position.y, transform.position.z), Quaternion.identity, gameObject.transform);
    }
}
