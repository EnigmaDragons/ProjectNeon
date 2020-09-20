
using UnityEngine;

[CreateAssetMenu(menuName = "AI/BossTheAggregateAI")]
public class BossTheAggregateAI : TurnAI
{
    private int _cooldownBeforeRepair;
    
    public override void InitForBattle()
    {
        _cooldownBeforeRepair = 4;
    }
    
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var me = battleState.Members[memberId];
        var componentsDestroyed = battleState.GetConsciousAllies(me).Length < 3;
        if (componentsDestroyed) 
            _cooldownBeforeRepair -= 1;
        
        var ctx = new CardSelectionContext(memberId, battleState, strategy);
        if (_cooldownBeforeRepair == 0)
        {
            ctx.WithSelectedCardByNameIfPresent("RepairComponents")
                .IfTruePlayType(_ => componentsDestroyed, CardTag.Attack);
            _cooldownBeforeRepair = 3;
        }

        return ctx
            .IfTruePlayType(_ => componentsDestroyed, CardTag.Attack)
            .IfTrueDontPlayType(_ => !componentsDestroyed, CardTag.Attack)
            .IfTrueDontPlayType(_ => !componentsDestroyed, CardTag.Ultimate)
            .WithSelectedTargetsPlayedCard();
    }
}
