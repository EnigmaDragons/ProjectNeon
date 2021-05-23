
public class CardConditionContext
{
    public Card Card { get; }
    public BattleState BattleState { get; }

    public CardConditionContext(Card c, BattleState b)
    {
        Card = c;
        BattleState = b;
    }
}