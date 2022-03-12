using System;
using System.Linq;
using MoreMountains.Feedbacks;
using UnityEngine;

public class BattleScreenJuice : OnMessage<EffectResolved>
{
    [SerializeField] private MMF_Player minorPositive;
    [SerializeField] private MMF_Player majorPositive;
    [SerializeField] private MMF_Player minorNegative;
    [SerializeField] private MMF_Player majorNegative;
    [SerializeField] private MMF_Player minorImpact;
    [SerializeField] private MMF_Player majorImpact;
    [SerializeField] private IntReference majorThreshold;
    
    protected override void Execute(EffectResolved msg)
    {
        if (msg.BattleAfter.Phase == BattleV2Phase.Setup || msg.BattleAfter.Phase == BattleV2Phase.NotBegun)
            return;
        
        var actionResultSummary = 0;
        var impactAmountSummary = 0;
        var enemyEffectiveHpChange = msg.SelectSum(x => x.TeamType == TeamType.Enemies ? x.State.HpAndShield : 0).Delta();
        var partyEffectiveHpChange = msg.SelectSum(x => x.TeamType == TeamType.Party ? x.State.HpAndShield : 0).Delta();

        if (enemyEffectiveHpChange < -majorThreshold.Value)
            actionResultSummary += 1;
        if (enemyEffectiveHpChange < 0)
            actionResultSummary += 1;
        if (enemyEffectiveHpChange > 0)
            actionResultSummary -= 1;
        if (enemyEffectiveHpChange > majorThreshold.Value)
            actionResultSummary -= 1;
        
        if (partyEffectiveHpChange < -majorThreshold.Value)
            actionResultSummary -= 1;
        if (partyEffectiveHpChange < 0)
            actionResultSummary -= 1;
        if (partyEffectiveHpChange > 0)
            actionResultSummary += 1;
        if (partyEffectiveHpChange > majorThreshold.Value)
            actionResultSummary += 1;


        actionResultSummary = actionResultSummary.Clamped(-2, 2);

        if (actionResultSummary == 1)
            minorPositive.PlayFeedbacks();
        if (actionResultSummary == 2)
            majorPositive.PlayFeedbacks();
        if (actionResultSummary == -1)
            minorNegative.PlayFeedbacks();
        if (actionResultSummary == -2)
            majorNegative.PlayFeedbacks();
        
        if (enemyEffectiveHpChange < 0)
            impactAmountSummary += 1;
        if (enemyEffectiveHpChange < -majorThreshold.Value)
            impactAmountSummary += 1;
        if (partyEffectiveHpChange < -majorThreshold.Value)
            impactAmountSummary += 1;
        if (partyEffectiveHpChange < 0)
            impactAmountSummary += 1;
        impactAmountSummary = impactAmountSummary.Clamped(0, 2);
        
        if (impactAmountSummary == 1)
            minorImpact.PlayFeedbacks();
        if (impactAmountSummary == 2)
            majorImpact.PlayFeedbacks();
        
        Log.Info($"EnemyHpChange: {enemyEffectiveHpChange}. PartyHpChange: {partyEffectiveHpChange}. ActionResult Summary - {actionResultSummary}. Impact Summary - {impactAmountSummary}");
    }
}
