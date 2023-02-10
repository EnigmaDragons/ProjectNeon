using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/NoEnemiesTargetedByOwnerThisTurn")]
public class NoEnemiesTargetedByOwnerThisTurn : StaticCardCondition
{
    [SerializeField] private CardTag[] tags;

    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.NoEnemy(enemy => ctx.BattleState.CurrentTurnCardPlays().Any(
            card => card.Member.Id == ctx.Card.Owner.Id
                && card.Targets.Any(
                    target => target.Members.Any(
                        x => x.Id == enemy.Id))
                && tags.All(x => card.Card.Tags.Contains(x))));
    
    public override string Description => $"No enemies have been targeted by owner this turn{(tags != null && tags.Any() ? $" with these tags: {string.Join(", ", tags.Select(x => x.ToString()))}" : "")}";
}