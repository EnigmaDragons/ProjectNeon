public class CardResolutionFinished
{
    public string CardName { get; }
    public int PlayedCardId { get; }
    public int CardId { get; }
    public int MemberId { get; }
    public int CardTypeId { get; }

    public CardResolutionFinished(IPlayedCard card)
        : this(card.Card.Name, card.Card.CardId, card.PlayedCardId, card.Member.Id, card.Card.Type.Id) {}
    
    public CardResolutionFinished(string cardName, int cardId, int playedCardId, int memberId, int cardTypeId)
    {
        CardName = cardName;
        PlayedCardId = playedCardId;
        CardId = cardId;
        MemberId = memberId;
        CardTypeId = cardTypeId;
    }
}
