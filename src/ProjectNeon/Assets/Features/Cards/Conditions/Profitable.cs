using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/Profitable")]
public class Profitable : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.BattleState.Party.Credits > ctx.BattleState.CreditsAtStartOfBattle;
    
    public override string Description => "Thoughts/Condition043".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition043" };
}