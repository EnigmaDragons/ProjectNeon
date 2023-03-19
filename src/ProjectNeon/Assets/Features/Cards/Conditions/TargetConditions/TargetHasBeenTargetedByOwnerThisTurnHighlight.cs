using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "TargetConditions/TargetedByOwner")]
public class TargetHasBeenTargetedByOwnerThisTurnHighlight : StaticTargetedCardCondition
{
    [SerializeField] private CardTag[] tags;

    public override bool ConditionMet(TargetedCardConditionContext ctx)
        => inversed != ctx.TargetIs(enemy => ctx.BattleState.CurrentTurnCardPlays().Any(
            card => card.Member.Id == ctx.Card.Owner.Id
                    && card.Targets.Any(
                        target => target.Members.Any(
                            x => x.Id == enemy.Id))
                    && tags.All(x => card.Card.Tags.Contains(x)))); 
}