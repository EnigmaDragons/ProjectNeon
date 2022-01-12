using UnityEngine;

[CreateAssetMenu(menuName = "AI/Specialist")]
public class TargetLeastHealthySpecialistAI : StatelessTurnAI
{
    protected override IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .IfTrueDontPlayType(x => true, CardTag.Attack)
            .WithCommonSenseSelections()
            .WithSelectedUltimateIfAvailable()
            .WithFinalizedCardSelection()
            .WithSelectedTargetsPlayedCard();
    }
}