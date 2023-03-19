using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/AllEnemiesTargetedByOwnerThisTurn")]
public class AllEnemiesTargetedByOwnerThisTurn : StaticCardCondition
{
    [SerializeField] private CardTag[] tags;

    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AllEnemies(enemy => ctx.BattleState.CurrentTurnCardPlays().Any(
            card => card.Member.Id == ctx.Card.Owner.Id
                    && card.Targets.Any(
                        target => target.Members.Any(
                            x => x.Id == enemy.Id))
                    && tags.All(x => card.Card.Tags.Contains(x))));

    public override string Description => "Thoughts/Condition006".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition006" };
}