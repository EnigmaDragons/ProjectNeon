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
                && tags.All(x => card.CardTags.Contains(x))));
    
    public override string Description => "Thoughts/Condition021".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition021" };
}