using System;
using System.Collections;
using System.Linq;
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
    [SerializeField] private CardPlayZone voidZone;

    public CardPlayZone DrawZone => drawZone;
    public CardPlayZone HandZone => handZone;
    public CardPlayZone PlayZone => playZone;
    public CardPlayZone DiscardZone => discardZone;
    public CardPlayZone SelectionZone => selectionZone;
    public CardPlayZone ResolutionZone => resolutionZone;
    public CardPlayZone VoidZone => voidZone;
    public Card[] AllCards => DrawZone.Cards.Concat(HandZone.Cards).Concat(DiscardZone.Cards).ToArray();
    
    public void ClearAll()
    {
        drawZone.Clear();
        handZone.Clear();
        playZone.Clear();
        discardZone.Clear();
        selectionZone.Clear();
        resolutionZone.Clear();
        reactionZone.Clear();
        voidZone.Clear();
        currentResolvingCardZone.Clear();
    }

    public void Reshuffle()
    {
        while (DiscardZone.HasCards) 
            DrawZone.PutOnBottom(DiscardZone.DrawOneCard());
        DrawZone.Shuffle();
    }
    
    public void DiscardHand()
    {
        while(HandZone.HasCards)
            DiscardZone.PutOnBottom(HandZone.DrawOneCard());
    }

    public void CycleHand()
    {
        var i = HandZone.Cards.Length;
        DiscardHand();
        DrawCards(i);
    }
    
    public IEnumerator DrawHandAsync(int handSize)
    {
        while (!HandZone.IsFull && HandZone.Count < handSize)
        {
            DrawOneCard();
            yield return new WaitForSeconds(0.3f);
        }
    }
    
    public void DrawCards(int number) 
        => Enumerable.Range(0, number).ForEach(_ => DrawOneCard());
    
    public void DrawCards(int number, Func<Card, bool> cardCondition)
    {
        Enumerable.Range(0, number).ForEach(_ => DrawOneCard(cardCondition));
    }

    public void DrawOneCard() => DrawOneCard(c => true);

    public void DrawOneCard(Func<Card, bool> cardCondition)
    {
        if (HandZone.IsFull)
        {
            BattleLog.Write("Hand Is Full. Not Drawing Any More Cards");
            return;
        }

        if (!DrawZone.Cards.Concat(DiscardZone.Cards).Any(cardCondition))
        {
            BattleLog.Write("Unable to find card of selected type. Not Drawing Any Cards");
            return;
        }

        if (!DrawZone.Cards.Any(cardCondition))
            Reshuffle();
        var card = DrawZone.DrawOneCard();
        HandZone.PutOnBottom(card);
        Message.Publish(new PlayerCardDrawn(card));
    }

    public void TestInit(params Card[] hand)
    {
        handZone = CardPlayZone.InMemory;
        handZone.Init(hand);
    }
    
    public void TestInitFull(Card[] draw, Card[] hand, Card[] discard)
    {
        drawZone = CardPlayZone.InMemory;
        drawZone.Init(draw);
        handZone = CardPlayZone.InMemory;
        handZone.Init(hand);
        discardZone = CardPlayZone.InMemory;
        discardZone.Init(discard);
    }

    public static CardPlayZones InMemory => (CardPlayZones)FormatterServices.GetUninitializedObject(typeof(CardPlayZones));
}
