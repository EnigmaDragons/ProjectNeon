using UnityEngine;

public class SetupPlayerCards : MonoBehaviour
{
    [SerializeField] private CardPlayZones PlayerCardPlayZones;
    [SerializeField] private Card DebugDefaultCard;

    // @todo #30:10min Create IntVariable Scriptable Object and setup StartingCards IntVariable

    private CardPlayZone Hand => PlayerCardPlayZones.HandZone;
    private CardPlayZone Discard => PlayerCardPlayZones.DiscardZone;
    private CardPlayZone Play => PlayerCardPlayZones.PlayZone;
    private CardPlayZone Deck => PlayerCardPlayZones.DrawZone;

    void Awake()
    {
        Hand.Clear();
        Discard.Clear();
        Play.Clear();
        Deck.Clear();
        for (var i = 0; i < 8; i++)
            Deck.PutOnBottom(DebugDefaultCard);
    }

    void Start()
    {
        for (var c = 0; c < 6; c++)
            Hand.PutOnBottom(Deck.DrawOneCard());
    }
}
