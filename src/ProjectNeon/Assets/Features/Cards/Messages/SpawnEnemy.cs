
using UnityEngine;

public class SpawnEnemy
{
    public Enemy Enemy { get; }
    public Vector3 Offset { get; }
    public Member Source { get; }
    public bool IsReplacing { get; }
    public Maybe<Card> Card { get; }
    public ResourceQuantity XPaidAmount { get; }
    public ResourceQuantity PaidAmount { get; }
    public bool IsReaction { get; }
    public ReactionTimingWindow Timing { get; }
    public Maybe<EffectCondition> Condition { get; }

    public SpawnEnemy(Enemy e, Vector3 offset, Member source, bool isReplacing, Maybe<Card> card, ResourceQuantity xPaidAmount, 
        ResourceQuantity paidAmount, bool isReaction, ReactionTimingWindow timing, Maybe<EffectCondition> condition)
    {
        Enemy = e;
        Offset = offset;
        Source = source;
        IsReplacing = isReplacing;
        Card = card;
        XPaidAmount = xPaidAmount;
        PaidAmount = paidAmount;
        IsReaction = isReaction;
        Timing = timing;
        Condition = condition;
    }
}
