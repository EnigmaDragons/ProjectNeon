
using System.Collections.Generic;

public class CardConditionContext
{
    public Card Card { get; }
    public BattleState BattleState { get; }
    public List<IPlayedCard> PendingCardsToResolve { get; }

    public CardConditionContext(Card c, BattleState b)
        : this(c, b, new List<IPlayedCard>()) {}
        
    public CardConditionContext(Card c, BattleState b, List<IPlayedCard> pendingCardsToResolve)
    {
        Card = c;
        BattleState = b;
        PendingCardsToResolve = pendingCardsToResolve;
    }
}