using UnityEngine;

[CreateAssetMenu(menuName = "AI/GeneralAI")]
public class GeneralAI : TurnAI
{
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var me = battleState.Members[memberId];
        
        return new CardSelectionContext(me, battleState, strategy)
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .WithCommonSenseSelections()
            .WithFinalizedCardSelection()
            .WithSelectedTargetsPlayedCard();
    }
}
