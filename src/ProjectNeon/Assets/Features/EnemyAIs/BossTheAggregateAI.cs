using UnityEngine;

[CreateAssetMenu(menuName = "AI/BossTheAggregateAI")]
public class BossTheAggregateAI : StatelessTurnAI
{
    protected override IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var me = battleState.Members[memberId];
        var componentsDestroyed = battleState.GetConsciousAllies(me).Length < 3;
        if (componentsDestroyed)
            me.Apply(m => m.AdjustPrimaryResource(1));
        
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedUltimateIfAvailable()
            .IfTruePlayType(_ => componentsDestroyed, CardTag.Attack)
            .IfTrueDontPlayType(_ => !componentsDestroyed, CardTag.Attack)
            .IfTrueDontPlayType(_ => !componentsDestroyed, CardTag.Ultimate)
            .WithFinalizedCardSelection()
            .WithSelectedTargetsPlayedCard();
    }
}
