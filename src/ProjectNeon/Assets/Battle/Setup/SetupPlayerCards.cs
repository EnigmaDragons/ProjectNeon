using UnityEngine;

public class SetupPlayerCards : MonoBehaviour
{
    [SerializeField] private CardPlayZones playerCardPlayZones;
    [SerializeField] private Card debugDefaultCard;
    [SerializeField] private GameEvent onFinished;
    [SerializeField] private IntReference startingCards;

    private CardPlayZone Hand => playerCardPlayZones.HandZone;
    private CardPlayZone Discard => playerCardPlayZones.DiscardZone;
    private CardPlayZone Play => playerCardPlayZones.PlayZone;
    private CardPlayZone Deck => playerCardPlayZones.DrawZone;

    void Awake()
    {
        Hand.Clear();
        Discard.Clear();
        Play.Clear();
        Deck.Clear();
        for (var i = 0; i < 8; i++)
            Deck.PutOnBottom(debugDefaultCard);
    }

    void Start()
    {
        Deck.Shuffle();
        for (var c = 0; c < startingCards.Value; c++)
            Hand.PutOnBottom(Deck.DrawOneCard());
        onFinished.Publish();
    }
}
