using UnityEngine;

[CreateAssetMenu(menuName = "AI/Striker")]
public sealed class StrikerAI : StatelessTurnAI
{
    protected override IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithCommonSenseSelections()
            .WithSelectedUltimateIfAvailable()
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .IfTrueDontPlayType(ctx => ctx.Member.HasAttackBuff(), CardTag.BuffAttack)
            .IfTrueDontPlayType(ctx => ctx.Member.HasDoubleDamage(), CardTag.DoubleDamage)
            .WithFinalizedCardSelection()
            .WithSelectedTargetsPlayedCard();
    }
}
