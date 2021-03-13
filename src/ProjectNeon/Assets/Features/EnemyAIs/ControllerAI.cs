using UnityEngine;

[CreateAssetMenu(menuName = "AI/Controller")]
public class ControllerAI : StatelessTurnAI
{
    protected override IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {        
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithCommonSenseSelections()
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .WithSelectedUltimateIfAvailable()
            .WithSelectedTargetsPlayedCard();
    }
}
