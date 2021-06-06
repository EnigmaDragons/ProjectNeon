using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/Profitable")]
public class Profitable : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.BattleState.Party.Credits > ctx.BattleState.CreditsAtStartOfBattle;
}