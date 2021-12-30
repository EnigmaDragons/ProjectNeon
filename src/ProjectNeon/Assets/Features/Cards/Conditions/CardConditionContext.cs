using System;
using System.Collections.Generic;
using System.Linq;

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

    public bool AnyEnemy(Func<Member, bool> condition) 
        => BattleState.GetConsciousEnemies(Card.Owner).Any(condition);
    
    public bool AllEnemies(Func<Member, bool> condition) 
        => BattleState.GetConsciousEnemies(Card.Owner).All(condition);

    public bool NoEnemy(Func<Member, bool> condition)
        => !AnyEnemy(condition);
}
