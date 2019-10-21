using System.Linq;
using UnityEngine;

public class SetupPlayerCards : MonoBehaviour
{
    [SerializeField] private Party party;
    [SerializeField] private CardPlayZones playerCardPlayZones;
    [SerializeField] private GameEvent onFinished;
    [SerializeField] private IntReference startingCards;

    private CardPlayZone Hand => playerCardPlayZones.HandZone;
    private CardPlayZone Discard => playerCardPlayZones.DiscardZone;
    private CardPlayZone Play => playerCardPlayZones.PlayZone;
    private CardPlayZone Deck => playerCardPlayZones.DrawZone;
    private CardPlayZone Selection => playerCardPlayZones.SelectionZone;

    void Awake()
    {
        playerCardPlayZones.ClearAll();
        Deck.Init(party.Decks.SelectMany(x => x.Cards));
    }

    void Start()
    {
        Deck.Shuffle();
        for (var c = 0; c < startingCards.Value; c++)
            Hand.PutOnBottom(Deck.DrawOneCard());
        onFinished.Publish();
    }
}
