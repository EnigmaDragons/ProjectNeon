using UnityEngine;

[CreateAssetMenu(menuName = "AI/BossTheAggregateAI")]
public class BossTheAggregateAI : TurnAI
{
    public override void InitForBattle()
    {
        BattleLog.Write("Initialized The Aggregate Core");
    }
    
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
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
