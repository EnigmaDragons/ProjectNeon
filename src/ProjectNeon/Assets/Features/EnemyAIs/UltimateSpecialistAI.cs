using UnityEngine;

[CreateAssetMenu(menuName = "AI/UltimateSpecialist")]
public class UltimateSpecialistAI : StatelessTurnAI
{
    protected override IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedUltimateIfAvailable()
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .IfTrueDontPlayType(x => true, CardTag.Attack)
            .WithCommonSenseSelections()
            .WithFinalizedCardSelection()
            .WithSelectedTargetsPlayedCard();
    }
}