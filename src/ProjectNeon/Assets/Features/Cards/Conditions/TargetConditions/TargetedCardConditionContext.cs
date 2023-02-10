using System;
using System.Linq;

public class TargetedCardConditionContext
{
    public Card Card { get; }
    public BattleState BattleState { get; }
    public Target PrimaryTarget { get; }

    public TargetedCardConditionContext(Card card, BattleState battleState, Target primaryTarget)
    {
        Card = card;
        BattleState = battleState;
        PrimaryTarget = primaryTarget;
    }

    public bool TargetIs(Func<Member, bool> condition)
        => PrimaryTarget.Members.All(condition);
}