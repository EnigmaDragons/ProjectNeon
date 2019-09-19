using UnityEngine;

public class DebugDrawCard : MonoBehaviour
{
    [SerializeField] private CardPlayZone Deck;
    [SerializeField] private CardPlayZone Hand;

    public void DrawOneCard()
    {
        Hand.PutOnBottom(Deck.DrawOneCard());
    }
}
