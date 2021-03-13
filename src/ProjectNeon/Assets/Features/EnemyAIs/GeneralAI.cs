using UnityEngine;

[CreateAssetMenu(menuName = "AI/GeneralAI")]
public class GeneralAI : StatelessTurnAI
{
    protected override IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {
        return new CardSelectionContext(battleState.Members[memberId], battleState, strategy)
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .WithCommonSenseSelections()
            .WithSelectedTargetsPlayedCard();
    }
}
