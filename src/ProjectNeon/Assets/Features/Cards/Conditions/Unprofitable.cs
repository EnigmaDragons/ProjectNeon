using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/Unprofitable")]
public class Unprofitable : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.BattleState.Party.Credits <= ctx.BattleState.CreditsAtStartOfBattle;
    
    public override string Description => "Thoughts/Condition044".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition044" };
}