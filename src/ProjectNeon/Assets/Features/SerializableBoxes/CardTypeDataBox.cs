using System;

[Serializable] 
public class CardTypeDataBox
{
    public Card Card;
    public CardType CardType;
    public ReactionCardType ReactionCardType;
    
    public CardTypeData Get()
    {
        
    }
    
    public void Set(CardTypeData cardTypeData)
    {
        if (cardTypeData == null)
            return;
        Card = null;
        CardType = null;
        ReactionCardType = null;
        if (cardTypeData is Card card)
            Card = card;
        else if ()
    }
}