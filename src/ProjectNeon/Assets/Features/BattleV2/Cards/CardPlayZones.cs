using System.Runtime.Serialization;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/CardPlayZones")]
public class CardPlayZones : ScriptableObject
{
    [SerializeField] private CardPlayZone drawZone;
    [SerializeField] private CardPlayZone handZone;
    [SerializeField] private CardPlayZone playZone;
    [SerializeField] private CardPlayZone discardZone;
    [SerializeField] private CardPlayZone selectionZone;
    [SerializeField] private CardPlayZone resolutionZone;
    [SerializeField] private CardPlayZone reactionZone;
    [SerializeField] private CardPlayZone currentResolvingCardZone;

    public CardPlayZone DrawZone => drawZone;
    public CardPlayZone HandZone => handZone;
    public CardPlayZone PlayZone => playZone;
    public CardPlayZone DiscardZone => discardZone;
    public CardPlayZone SelectionZone => selectionZone;

    public void ClearAll()
    {
        drawZone.Clear();
        handZone.Clear();
        playZone.Clear();
        discardZone.Clear();
        selectionZone.Clear();
        resolutionZone.Clear();
        reactionZone.Clear();
        currentResolvingCardZone.Clear();
    }

    public void Reshuffle()
    {
        while (DiscardZone.Count > 0) 
            DrawZone.PutOnBottom(DiscardZone.DrawOneCard());
        DrawZone.Shuffle();
    }

    public void DrawHand(int handSize)
    {
        for (var c = 0; c < handSize; c++)
            HandZone.PutOnBottom(DrawZone.DrawOneCard());
    }

    public static CardPlayZones InMemory => (CardPlayZones)FormatterServices.GetUninitializedObject(typeof(CardPlayZones));
}
