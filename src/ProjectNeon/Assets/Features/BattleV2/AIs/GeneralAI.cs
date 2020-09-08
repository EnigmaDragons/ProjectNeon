using UnityEngine;

[CreateAssetMenu(menuName = "AI/GeneralAI")]
public class GeneralAI : TurnAI
{
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var me = battleState.Members[memberId];
        var playableCards = battleState.GetPlayableCards(memberId);
        
        var ctx = new CardSelectionContext(me, battleState, strategy)
            .WithOptions(playableCards)
            .WithSelectedDesignatedAttackerCardIfApplicable();

        return ctx.WithFinalizedCardSelection()
            .WithSelectedTargetsPlayedCard();
    }
}
