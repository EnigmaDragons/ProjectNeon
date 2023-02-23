
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/NoPlayerHandCardsAreGlitched")]
public class NoPlayerHandCardsAreGlitched : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx) 
        => ctx.BattleState.PlayerCardZones.HandZone.Cards.None(x => x.Mode == CardMode.Glitched);
    
    public override string Description => "Thoughts/Condition025".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition025" };
}
