using System;

[Serializable] 
public class CardTypeDataBox
{
    public Card Card;
    public CardType CardType;
    public ReactionCardType ReactionCardType;
    
    public CardTypeDataBox() {}

    public CardTypeDataBox(CardTypeData card)
    {
        Set(card);
    }

    public bool IsBoxFilled() => Get() != null;
    
    public CardTypeData Get()
    {
        if (Card != null)
            return Card;
        else if (CardType != null)
            return CardType;
        else if (ReactionCardType != null)
            return ReactionCardType;
        else return null;
    }

    public void Set(CardTypeData cardTypeData)
    {
        if (cardTypeData == null || cardTypeData is InMemoryCard)
            return;
        Card = null;
        CardType = null;
        ReactionCardType = null;
        if (cardTypeData is Card card)
            Card = card;
        else if (cardTypeData is CardType cardType)
            CardType = cardType;
        else if (cardTypeData is ReactionCardType reactionCardType)
            ReactionCardType = reactionCardType;
    }
}